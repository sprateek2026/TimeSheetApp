using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Timesheet.Api.Models
{
    [Table("TM_DAILYSUMMARIES")]
    public class DailySummary
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int USERID { get; set; }

        [Required]
        public int SUBTASKID { get; set; }

        [Required]
        public DateTime WORKDATE { get; set; }

        [Required]
        public decimal TOTALDAILYHR { get; set; }

        [Required]
        public int APPROVALSTATUS { get; set; }

        public int? APPROVEDBY { get; set; }

        public DateTime? APPROVALDATE { get; set; }

        public bool? RAISEHANDFLAG { get; set; }

        public int? INVOICEID { get; set; }
    }
}
