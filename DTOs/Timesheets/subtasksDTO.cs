namespace Timesheet.Api.Entities
{
    public class subtasksDTO
    {
        public int id { get; set; }
        public int taskid { get; set; }
        public string subtask { get; set; }
        public string description { get; set; }
        public bool isactive { get; set; }
    }
}
