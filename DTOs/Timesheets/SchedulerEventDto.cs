namespace Timesheet.Api.DTOs.Timesheets
{
    public class SchedulerEventDto
    {
        public int TimeLogId { get; set; }
        public string Title { get; set; }
        public int customerId { get; set; }
        public int projectId { get; set; }
        public int phaseId { get; set; }
        public int subPhaseId { get; set; }
        public int taskId { get; set; }
        //public int subTaskId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public int Approvalstatus { get; set; }
        public int? Approvedby { get; set; }
        //public ICollection<TmTimeLogDetail> TIMELOGS { get; set; } = new List<TmTimeLogDetail>();
        public DateTime? Approvaldate { get; set; }
        public bool IsAllDay { get; set; }
        public int SummaryId { get; set; }
        public int SubTaskId { get; set; }
    }
}
