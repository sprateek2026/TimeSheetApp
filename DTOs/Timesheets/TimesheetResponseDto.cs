namespace Timesheet.Api.DTOs.Timesheets
{
    public class TimesheetResponseDto
    {
        public int SummaryId { get; set; }
        public int UserId { get; set; }
        public DateTime WorkDate { get; set; }
        public decimal TotalHours { get; set; }
        public int ApprovalStatus { get; set; }
        public List<TimeLogDto> TimeLogs { get; set; } = new();
    }

    public class TimeLogDto
    {
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
        public decimal LoggedHours { get; set; }
        public int SubtaskId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
