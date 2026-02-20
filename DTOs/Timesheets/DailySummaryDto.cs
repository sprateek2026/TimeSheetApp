namespace Timesheet.Api.DTOs.Timesheets
{
    public class DailySummaryDto
    {
        public int DailySummariesId { get; set; }
        public int UserId { get; set; }
        public int SubTaskId { get; set; }
        public string WorkDate { get; set; }
        public decimal TotalDailyHr { get; set; }
        public bool RaiseHandFlag { get; set; }
        public int? InvoiceId { get; set; }
    }

}
