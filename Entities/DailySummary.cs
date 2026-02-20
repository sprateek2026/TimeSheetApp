using Timesheet.Api.Entities;
using Timesheet.Api.Enums;

public class DailySummary
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SubTaskId { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal TotalDailyHr { get; set; }
    public int? ApprovedBy { get; set; }     // ✅ ADD
    public DateTime? ApprovalDate { get; set; } // ✅ ADD
    public TimesheetStatus ApprovalStatus { get; set; }

    public AppUser User { get; set; } = null!;
}
