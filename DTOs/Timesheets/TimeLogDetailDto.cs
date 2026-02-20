namespace Timesheet.Api.DTOs.Timesheets
{
    public class TimeLogDetailDto
    {
        public int TimeLogDetailsId { get; set; }
        public int SummaryId { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
        public decimal LoggedHr { get; set; }
        public int SubTaskId { get; set; }
        public string Description { get; set; }
        public int ApprovalStatus { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
    }

}
