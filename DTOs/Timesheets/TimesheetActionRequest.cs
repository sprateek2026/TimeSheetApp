namespace Timesheet.Api.DTOs.Timesheets
{
    public class TimesheetActionRequest
    {
        public int Id { get; set; }
        public int SummaryId { get; set; }

        public int Action { get; set; }
        public int ActionByUserId { get; set; } // Manager / Employee
        public string? Comment { get; set; }
    }
}
