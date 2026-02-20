using System.ComponentModel.DataAnnotations.Schema;

namespace Timesheet.Api.Entities
{
    public class TM_TASKS
    {
        public int ID { get; set; }
        public int SUBPHASEID { get; set; }
        public string TASK { get; set; }
        public string DESCRIPTION { get; set; }
        public bool ISACTIVE { get; set; }

        [ForeignKey(nameof(SUBPHASEID))]
        public TM_SUBPHASE SubPhase { get; set; }
    }
}
