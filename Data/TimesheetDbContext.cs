using Microsoft.EntityFrameworkCore;
using Timesheet.Api.Entities;

namespace Timesheet.Api.Data
{
    public class TimesheetDbContext : DbContext
    {
        public TimesheetDbContext(DbContextOptions<TimesheetDbContext> options)
            : base(options) { }

        //Master Tables
        public DbSet<TM_CUSTOMERS> TM_CUSTOMERS { get; set; }
        public DbSet<TM_PROJECTS> TM_PROJECTS { get; set; }
        public DbSet<TM_PHASE> TM_PHASE { get; set; }
        public DbSet<TM_SUBPHASE> TM_SUBPHASE { get; set; }
        public DbSet<TM_TASKS> TM_TASKS { get; set; }
        public DbSet<TM_SUBTASKS> TM_SUBTASKS { get; set; }
        public DbSet<TM_DAILYSUMMARY> TM_DAILYSUMMARY => Set<TM_DAILYSUMMARY>();
        public DbSet<TM_TIMELOGDETAILS> TM_TIMELOGDETAILS => Set<TM_TIMELOGDETAILS>();
        /// <summary>
        /// actionable entities
        /// </summary>

        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<TM_APPUSERS> TM_APPUSERS => Set<TM_APPUSERS>();
        //public DbSet<DailySummary> DailySummaries => Set<DailySummary>();

        public DbSet<TM_COMMENTS> TM_COMMENTS { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TM_TIMELOGDETAILS>()
            .HasOne(x => x.SubTask)
            .WithMany()
            .HasForeignKey(x => x.SUBTASKID);

                modelBuilder.Entity<TM_TIMELOGDETAILS>()
                    .HasOne(x => x.Summary)
                    .WithMany()
                    .HasForeignKey(x => x.SUMMARYID);
            //modelBuilder.Entity<DailySummary>()
            //    .ToTable("TM_DAILYSUMMARIES");
            modelBuilder.Entity<TM_DAILYSUMMARY>(entity =>
            {
                entity.ToTable("TM_DAILYSUMMARIES");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.TOTALDAILYHR)
                      .HasColumnName("TOTALDAILYHR");

                //entity.Property(e => e.APPROVALSTATUS)
                //      .HasConversion<int>();
            });

            //modelBuilder.Entity<TmDailySummary>()
            //    .ToTable("TM_DAILYSUMMARIES");

            modelBuilder.Entity<TM_TIMELOGDETAILS>()
                .ToTable("TM_TIMELOGDETAILS");

            modelBuilder.Entity<TM_PHASE>()
            .HasOne(p => p.Project)
            .WithMany()
            .HasForeignKey(p => p.PROJID);



            //modelBuilder.Entity<DailySummary>(entity =>
            //{
            //    entity.ToTable("TM_DAILYSUMMARIES"); // exact table name
            //    entity.HasKey(e => e.Id);
            //});
            //modelBuilder.Entity<TmDailySummary>(entity =>
            //{
            //    entity.ToTable("TM_DAILYSUMMARIES");
            //    entity.HasKey(e => e.Id);

            //    entity.Property(e => e.TotalDailyHr)
            //          .HasColumnName("TOTALDAILYHR");

            //    entity.Property(e => e.ApprovalStatus)
            //          .HasConversion<int>();
            //});

            //modelBuilder.Entity<TmTimeLogDetail>(entity =>
            //{
            //    entity.ToTable("TM_TIMELOGDETAILS");
            //    entity.HasKey(e => e.Id);

            //    entity.HasOne(d => d.Summary)
            //          .WithMany(p => p.TimeLogs)
            //          .HasForeignKey(d => d.SummaryId);
            //});
        }
    }


}
