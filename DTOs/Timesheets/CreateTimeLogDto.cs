namespace TimeSheet.Api.Dtos.Timesheets
{
    public class CreateTimeLogDto
    {
        public int SummaryId { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
        public decimal LoggedHours { get; set; }
        public int SubtaskId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
