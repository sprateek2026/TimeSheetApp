namespace Timesheet.Api.DTOs.Timesheets
{
    public class TimesheetMonthDto
    {
        public int Id { get; set; }           // task grouping id
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int? ManagerId { get; set; }

        public int CustomersId { get; set; }
        public string CustomersName { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public int PhaseId { get; set; }
        public string PhaseName { get; set; }

        public int SubPhaseId { get; set; }
        public string SubPhaseName { get; set; }

        public int TaskId { get; set; }
        public string TaskName { get; set; }

        public int SubTaskId { get; set; }
        public string SubTaskName { get; set; }

        public List<DailySummaryDto> DailySummaries { get; set; } = new();
        public List<TimeLogDetailDto> TimeLogDetails { get; set; } = new();
    }

}
