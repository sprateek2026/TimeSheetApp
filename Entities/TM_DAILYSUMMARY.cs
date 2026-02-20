using Timesheet.Api.Enums;

namespace Timesheet.Api.Entities
{
    public class TM_DAILYSUMMARY
    {
        public int ID { get; set; }
        public int USERID { get; set; }
        //public int SUBTASKID { get; set; }
        public DateTime WORKDATE { get; set; }
        public decimal TOTALDAILYHR { get; set; }
        //public TimesheetStatus APPROVALSTATUS { get; set; }
        

        public bool? RAISEHANDFLAG { get; set; }

        public int? INVOICEID { get; set; }
        public TM_APPUSERS User { get; set; } = null!;
    }
}
