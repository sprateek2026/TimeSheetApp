using System.Collections.Generic;

namespace Timesheet.Api.Entities
{
    public class AppUser
    {
        public int Id { get; set; }

        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public int DesignationId { get; set; }
        public int DisplayDesignationId { get; set; }
        public int DepartmentId { get; set; }

        public string UserRole { get; set; } = null!;
        public int ManagerId { get; set; }
        public string EmpType { get; set; } = null!;
        public bool IsActive { get; set; } = true;   // ✅ ADD THIS

        /* Navigation */
        public AppUser? Manager { get; set; }
        public ICollection<AppUser> Subordinates { get; set; } = new List<AppUser>();

        public ICollection<DailySummary> DailySummaries { get; set; } = new List<DailySummary>();
    }
}
