using Timesheet.Api.Enums;

namespace TimeSheet.Api.Dtos.Timesheets
{
    public class CreateTimesheetDto
    {
        public int UserId { get; set; }
        public DateTime WorkDate { get; set; }

        public int SubTaskId { get; set; }

        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }

        public decimal TotalDailyHr { get; set; }
        public string Description { get; set; } = string.Empty;

        public TimesheetStatus ApprovalStatus { get; set; }
    }

}
