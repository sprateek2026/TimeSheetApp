namespace Timesheet.Api.DTOs.Timesheets
{
    public class ManagerTimesheetDto
    {
        public int SummaryId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime WorkDate { get; set; }
        public decimal TotalHours { get; set; }
        public int Status { get; set; }
    }
}
