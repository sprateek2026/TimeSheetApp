namespace Timesheet.Api.Entities
{
    public class TM_TIMELOGDETAILS
    {
        public int  ID { get; set; }
        public int SUMMARYID { get; set; }
        public int CUSTOMERID { get; set; }
        public int PROJECTID { get; set; }
        public int PHASEID { get; set; }
        public int SUBPHASEID { get; set; }
        public int TASKID { get; set; }
        public int SUBTASKID { get; set; }
        public TimeSpan TIMEFROM { get; set; }
        public TimeSpan TIMETO { get; set; }
        public decimal LOGGEDHOURS { get; set; }

        public int APPROVALSTATUS { get; set; }

        public int? APPROVEDBY { get; set; }
        //public ICollection<TmTimeLogDetail> TIMELOGS { get; set; } = new List<TmTimeLogDetail>();
        public DateTime? APPROVALDATE { get; set; }

        public string DESCRIPTION { get; set; } = string.Empty;

        public TM_DAILYSUMMARY Summary { get; set; } = null!;
        public TM_SUBTASKS SubTask { get; set; } = null!;
    }
}
