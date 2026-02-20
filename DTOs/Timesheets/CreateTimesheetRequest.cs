using Timesheet.Api.Enums;

namespace Timesheet.Api.DTOs.Timesheets
{
    public class CreateTimesheetRequest
    {
        public int UserId { get; set; }
        public DateTime WorkDate { get; set; }

        // Draft / Submitted only allowed on create
        public TimesheetStatus ApprovalStatus { get; set; }

        public List<TimeLogItemDto> TimeLogs { get; set; } = new();
    }
}
