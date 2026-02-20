using System.ComponentModel.DataAnnotations.Schema;

namespace Timesheet.Api.Entities
{
    public class TM_PHASE
    {
        public int ID { get; set; }
        public int PROJID { get; set; }
        public string PHASECODE { get; set; }
        public string DESCRIPTION { get; set; }
        public bool ISACTIVE { get; set; }

        [ForeignKey(nameof(PROJID))]
        public TM_PROJECTS Project { get; set; }
        
    }
}
