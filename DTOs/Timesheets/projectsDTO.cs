using Timesheet.Api.Enums;

namespace Timesheet.Api.DTOs.Timesheets
{
    public class projectsDTO
    {
        public int id { get; set; }
        public int customerid { get; set; }
        public string projcode { get; set; }
        public string description { get; set; }
        public bool isactive { get; set; }
    }
}
