namespace Timesheet.Api.DTOs.Timesheets
{
    public class SaveTimeLogRequestDTO
    {
        public int? Id { get; set; }              // null = create, value = update
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public int ProjectId { get; set; }
        public int PhaseId { get; set; }
        public int SubPhaseId { get; set; }
        public int TaskId { get; set; }
        public int SubTaskId { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public decimal LoggedHours { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
