using Timesheet.Api.DTOs.Login;
using Timesheet.Api.DTOs.Timesheets;
using Timesheet.Api.Entities;
using Timesheet.Api.Models;
using TimeSheet.Api.Dtos.Timesheets;

namespace Timesheet.Api.Services
{
    public interface ITimesheetService
    {
        //Masters
        Task<List<CustomerDTO>> GetCustomersAsync();
        Task<List<projectsDTO>> GetProjectsAsync(int custId);
        Task<List<phaseDTO>> GetPhasesAsync(int projId);
        Task<List<subphaseDTO>> GetSubPhasesAsync(int phaseId);

        Task<List<tasksDTO>> GetTasksAsync(int subPhaseId);
        Task<List<subtasksDTO>> GetSubTasksAsync(int taskId);

        //Actions
        Task<List<TM_DAILYSUMMARY>> GetDailySummuryByUserAsync(int userId);
        Task<(List<SchedulerEventDto> Data, int Total)>  GetSchedulerEventsAsync(
            //DateTime from, 
            //DateTime to, 
            int userId);

        Task SaveOrUpdateTimeLogAsync(SaveTimeLogRequestDTO request);

        Task DeleteTimeLogAsync(int id);

        Task<List<TimesheetMonthDto>> GetMonthlyTimesheetAsync( int userId, int year, int month);

        Task ActionOnTimeLogAsync(TimesheetActionRequest request);

        Task<TM_APPUSERS> ValidateLoginAsync(LoginRequest request);
        //----------------------------------------------end used code------------------------------------------------//
        Task<TM_DAILYSUMMARY> AddAsync(TM_DAILYSUMMARY summary);

        Task<int> CreateDraftAsync(CreateTimesheetDto dto);
        Task SubmitAsync(int timesheetId, int userId);
        Task ApproveAsync(int timesheetId, int managerId);
        Task RejectAsync(int timesheetId, int managerId, string reason);

        Task SubmitTimesheetAsync(int summaryId);
        
        Task RejectTimesheetAsync(TimesheetActionRequest request);

        Task<int> CreateTimesheetAsync(CreateTimesheetRequest request);

        Task<List<TimesheetResponseDto>> GetTimesheetsByUserAsync(int userId);
        Task<List<TimesheetResponseDto>> GetTimesheetsByDateRangeAsync(
            DateTime fromDate, DateTime toDate);

        //Task<TimesheetResponseDto?> GetTimesheetByIdAsync(int summaryId);
        Task<List<ManagerTimesheetDto>> GetPendingForManagerAsync(int managerId);


    }
}
