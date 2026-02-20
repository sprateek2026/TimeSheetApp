using System.ComponentModel.DataAnnotations.Schema;

namespace Timesheet.Api.Entities
{
    public class TM_PROJECTS
    {
        public int ID { get; set; }
        public int CUSTOMERID { get; set; }
        public string PROJCODE { get; set; }
        public string DESCRIPTION { get; set; }
        public bool ISACTIVE { get; set; }

        [ForeignKey(nameof(CUSTOMERID))]
        public TM_CUSTOMERS Customer { get; set; }
    }
}
