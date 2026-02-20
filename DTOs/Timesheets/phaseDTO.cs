using Timesheet.Api.Enums;

namespace Timesheet.Api.DTOs.Timesheets
{
    public class phaseDTO
    {
        public int id { get; set; }
        public int projid { get; set; }
        public string phasecode { get; set; }
        public string description { get; set; }
        public bool isactive { get; set; }
    }
}
