namespace Timesheet.Api.Entities
{
    public class tasksDTO
    {
        public int id { get; set; }
        public int subphaseid { get; set; }
        public string task { get; set; }
        public string description { get; set; }
        public bool isactive { get; set; }
    }
}
