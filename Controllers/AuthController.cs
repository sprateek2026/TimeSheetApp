using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Timesheet.Api.DTOs.Login;
using Timesheet.Api.Services;


namespace Timesheet.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ITimesheetService _service;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config, ITimesheetService service)
        {
            _config = config;
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            //System.Diagnostics.Debugger.Break(); // 🔥 MUST HIT
            var data= await _service.ValidateLoginAsync(request);
            // TEMP: Replace with DB validation
           // if (request.Username != "admin" && request.Password != "1234")
           if(data ==null)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(request.email, data.ID.ToString());
            return Ok(new
            {
                token,
               // userId = data.ID,
                userName = data.EMAIL,
                role =data.USERROLE,
                Name=data.FNAME+' '+data.LNAME
            }); 
        }


        private string GenerateJwtToken(string email, string userId)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
