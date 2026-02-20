using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Timesheet.Api.Data;
using Timesheet.Api.Services;
using Timesheet.Api.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;//Added for the version
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Timesheet.Api.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

var jwtSettings = builder.Configuration.GetSection("Jwt");
// Add services to the container.

builder.Services.AddControllers();

// Add API Versioning (default to v2.0)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    // Support versioning via query string, header, or URL segment
    options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("x-api-version"),
            new UrlSegmentApiVersionReader()
    );
});

// Add versioned API explorer (for Swagger grouping)
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings["Key"]))
            };
        });

builder.Services.AddAuthorization();

builder.Services.AddValidatorsFromAssemblyContaining<CreateTimeLogDtoValidator>();
builder.Services.AddScoped<HierarchyService>();
builder.Services.AddFluentValidationAutoValidation();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger and add JWT bearer support in the UI
builder.Services.AddSwaggerGen(c =>
{
    // Keep v1 doc for compatibility, add v2
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Timesheet API", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "Timesheet API", Version = "v2" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                {
                        new OpenApiSecurityScheme
                        {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] { }
                }
        });

    c.MapType<TimeOnly>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "time"
    });

    c.MapType<DateOnly>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
});

builder.Services.AddDbContext<TimesheetDbContext>(options =>
        options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
        ));
builder.Services.AddScoped<ITimesheetService, TimesheetService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        //policy
        //        .WithOrigins("http://localhost:4200")
        //        .AllowAnyHeader()
        //        .AllowAnyMethod();

        policy
            //.WithOrigins("http://localhost:4200")
            //.WithOrigins(
            //    "http://localhost:8000",
            //    "http://192.168.1.124:8000"
            //)
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();


// Configure Swagger UI to expose versioned endpoints
//if (app.Environment.IsDevelopment())
//{
//    // Ensure the versioned api explorer is available
//    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        // Expose only v2 and v1 docs (v2 preferred). The list is explicit so v2 appears.
//        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Timesheet API v1");
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Timesheet API v2");
//    });
//}

// IMPORTANT for IIS virtual path
app.UsePathBase("/TimeSheetManagement");

// Enable Swagger in IIS
app.UseSwagger();
app.UseSwaggerUI(options =>
{
        options.SwaggerEndpoint("/TimeSheetManagement/swagger/v2/swagger.json", "Timesheet API v2");
        options.SwaggerEndpoint("/TimeSheetManagement/swagger/v1/swagger.json", "Timesheet API v1");

        options.RoutePrefix = string.Empty; // 🔥 This makes Swagger load at root
});


// -------------------- MIDDLEWARE --------------------

// Configure the HTTP request pipeline.
// 🔥 ADD ROUTING (THIS IS THE MISSING PIECE)
app.UseRouting();
app.UseCors("AllowAngular");   // 🔥 MUST be here
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.MapControllers();
try
{
    // 🔥 APPLY MIGRATIONS
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider
                             .GetRequiredService<TimesheetDbContext>();

        dbContext.Database.Migrate();
    }

    Log.Information("Starting application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
