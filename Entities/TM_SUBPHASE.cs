using System.ComponentModel.DataAnnotations.Schema;

namespace Timesheet.Api.Entities
{
    public class TM_SUBPHASE
    {
        public int ID { get; set; }
        public int PHASEID { get; set; }
        public string SUBPHASECODE { get; set; }
        public string DESCRIPTION { get; set; }
        public bool ISACTIVE { get; set; }
        [ForeignKey(nameof(PHASEID))]
        public TM_PHASE Phase { get; set; }
    }
}
