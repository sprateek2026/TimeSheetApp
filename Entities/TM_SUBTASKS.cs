using System.ComponentModel.DataAnnotations.Schema;

namespace Timesheet.Api.Entities
{
    public class TM_SUBTASKS
    {
        public int ID { get; set; }
        public string SUBTASK { get; set; }
        public int TASKID { get; set; }
        public string DESCRIPTION { get; set; }
        public bool ISACTIVE { get; set; }
        [ForeignKey(nameof(TASKID))]
        public TM_TASKS Task { get; set; }
    }

}
