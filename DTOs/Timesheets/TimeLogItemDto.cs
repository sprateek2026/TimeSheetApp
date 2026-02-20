namespace Timesheet.Api.DTOs.Timesheets
{
    public class TimeLogItemDto
    {
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
        public decimal LoggedHours { get; set; }
        public int SubtaskId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
