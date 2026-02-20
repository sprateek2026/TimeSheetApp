using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Timesheet.Api.DTOs.Timesheets;
using Timesheet.Api.Models;
using Timesheet.Api.Services;
using TimeSheet.Api.Dtos.Timesheets;


namespace Timesheet.Api.Controllers
{
    [ApiController]
    [Route("api/timesheets")]
    public class TimesheetsController : ControllerBase
    {
        private readonly ITimesheetService _service;
        private readonly ILogger<TimesheetService> _logger;

        public TimesheetsController(ITimesheetService service, ILogger<TimesheetService> logger)
        {
            _service = service;
            _logger = logger;
        }

        protected int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("UserId claim not found");

            if (!int.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid UserId claim");

            return userId;
        }

        [Authorize]
        [HttpGet("Customers")]
        public async Task<IActionResult> GetCustomers()
        {
            _logger.LogInformation("get customer data by user {UserId}", GetUserId());
            var result = await _service.GetCustomersAsync();
            if (result == null)
                throw new KeyNotFoundException("Data not found");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("projects/{custId}")]
        public async Task<IActionResult> GetProjects(int custId)
        {
            _logger.LogInformation("GetProjects data by user {UserId}", GetUserId());
            var result = await _service.GetProjectsAsync(custId);
            if (result == null)
                throw new KeyNotFoundException("Data not found");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("phases/{projId}")]
        public async Task<IActionResult> GetPhases(int projId)
        {
            _logger.LogInformation("GetPhases data by user {UserId}", GetUserId());
            var result = await _service.GetPhasesAsync(projId);
            if (result == null)
                throw new KeyNotFoundException("Data not found");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("subphases/{phaseId}")]
        public async Task<IActionResult> GetSubPhases(int phaseId)
        {
            var result = await _service.GetSubPhasesAsync(phaseId);
            if (result == null)
                throw new KeyNotFoundException("Data not found");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("tasks/{subPhaseId}")]
        public async Task<IActionResult> GetTasks(int subPhaseId)
        {
            var result = await _service.GetTasksAsync(subPhaseId);
            if (result == null)
                throw new KeyNotFoundException("Data not found");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("subTasks/{taskId}")]
        public async Task<IActionResult> GetSubTasks(int taskId)
        {
            var result = await _service.GetSubTasksAsync(taskId);
            if (result == null)
                throw new KeyNotFoundException("Data not found");
            return Ok(result);
        }

        //----------------------------------------------

        [Authorize]
        // GET api/timesheets/user/3
        [HttpGet("DailySummury/user/{userId}")]
        public async Task<IActionResult> GetDailySummuryByUser(int userId)
        {
            var result = await _service.GetDailySummuryByUserAsync(userId);
            if (result == null)
                throw new KeyNotFoundException("Data not found");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetTimeSheetData/user")]
        public async Task<IActionResult> Read(
            //[FromQuery] DateTime from,
            //[FromQuery] DateTime to,
            //[FromRoute] int userId
            )
        {
            // Optionally, you can parse to int if needed:
            //int userIdClaimInt = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var val) ? val : 0;
            var result = await _service.GetSchedulerEventsAsync(
                //from, 
                //to, 
                GetUserId());
            //if (result == null)
            //    throw new KeyNotFoundException("Data not found");

            return Ok(new
            {
                data = result.Data,
                total = result.Total
            });
        }

        [Authorize]
        [HttpPost("SaveTimeLog")]
        public async Task<IActionResult> SaveTimeLog(
        [FromBody] SaveTimeLogRequestDTO request)
        {
            int userIdClaimInt = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var val) ? val : 0;
            request.UserId = userIdClaimInt; // Ensure the UserId is set from the token
            await _service.SaveOrUpdateTimeLogAsync(request);
            return Ok();
        }

        [Authorize]
        [HttpDelete("DeleteTimeLog/{id}")]
        public async Task<IActionResult> DeleteTimeLog(int id)
        {
            await _service.DeleteTimeLogAsync(id);
            return Ok();
        }

        [Authorize]
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyTimesheet(
                    //int userId, 
                    int year, 
                    int month)
        {
            // Optionally, you can parse to int if needed:
            int userIdClaimInt = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var val) ? val : 0;
            var data = await _service.GetMonthlyTimesheetAsync(userIdClaimInt, year, month);
            return Ok(data);
        }

        [Authorize]
        // Approve
        [HttpPost("ActionOnTimeLog")]
        public async Task<IActionResult> ActionOnTimeLog([FromBody] TimesheetActionRequest request)
        {
            int userIdClaimInt = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var val) ? val : 0;
            request.ActionByUserId = userIdClaimInt; // Ensure the UserId is set from the token
            await _service.ActionOnTimeLogAsync(request);
            return Ok("action " + request.Action + " is successfully updated");
        }

        //----------------------------------------------end used code------------------------------------------------//


        //// POST api/timesheets
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] DailySummary model)
        //{
        //    var created = await _service.AddAsync(model);
        //    return CreatedAtAction(nameof(GetByUser), new { userId = model.UserId }, created);
        //}
        //[HttpPost("submit")]
        //public IActionResult CreateTimesheet([FromBody] CreateTimesheetDto dto)
        //{
        //    // Dummy object (echo back)
        //    var result = new
        //    {
        //        Id = 1,
        //        dto.UserId,
        //        dto.SubtaskId,
        //        dto.WorkDate,
        //        dto.TotalDailyHr,
        //        Status = "Saved Successfully"
        //    };

        //    return Ok(result);
        //}

        // 4.2 Date range

        [HttpGet("range")]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var result = await _service.GetTimesheetsByDateRangeAsync(from, to);
            return Ok(result);
        }

        //// 4.4 Single detail
        //[HttpGet("{summaryId}")]
        //public async Task<IActionResult> GetById(int summaryId)
        //{
        //    var result = await _service.GetTimesheetByIdAsync(summaryId);
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}

        [HttpPost]
        public async Task<IActionResult> Create(CreateTimesheetDto dto)
        {
            var id = await _service.CreateDraftAsync(dto);
            return Ok(new { TimesheetId = id });
        }
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateTimesheetDto dto)
        //{
        //    await _service.CreateDraftAsync(dto);
        //    return Ok(new { message = "Timesheet saved successfully" });
        //}

        [HttpPost("{id}/submit")]
        public async Task<IActionResult> Submit(int id, [FromQuery] int userId)
        {
            await _service.SubmitAsync(id, userId);
            return Ok();
        }
        // Submit
        [HttpPost("{summaryId}/submit1")]
        public async Task<IActionResult> Submit1(int summaryId)
        {
            await _service.SubmitTimesheetAsync(summaryId);
            return Ok("Submitted successfully");
        }
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromQuery] int managerId)
        {
            await _service.ApproveAsync(id, managerId);
            return Ok();
        }
        

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromQuery] int managerId, [FromBody] string reason)
        {
            await _service.RejectAsync(id, managerId, reason);
            return Ok();
        }
        // Reject
        [HttpPost("reject1")]
        public async Task<IActionResult> Reject1([FromBody] TimesheetActionRequest request)
        {
            await _service.RejectTimesheetAsync(request);
            return Ok("Rejected successfully");
        }
        [HttpGet("manager/{managerId}/pending")]
        public async Task<IActionResult> GetPendingForManager(int managerId)
        {
            var result = await _service.GetPendingForManagerAsync(managerId);
            return Ok(result);
        }
    }
}
