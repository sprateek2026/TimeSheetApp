namespace Timesheet.Api.DTOs.Timesheets
{
    public class subphaseDTO
    {
        public int id { get; set; }
        public int phaseid { get; set; }
        public string subphasecode { get; set; }
        public string description { get; set; }
        public bool isactive { get; set; }
    }
}
