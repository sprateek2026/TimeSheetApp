using Microsoft.AspNetCore.Mvc;
using TimeSheet.Api.Dtos.Timesheets;

[ApiController]
[Route("api/timelogs")]
public class TimeLogsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateTimeLog([FromBody] CreateTimeLogDto dto)
    {
        var response = new
        {
            Id = 1,
            dto.SummaryId,
            dto.TimeFrom,
            dto.TimeTo,
            dto.LoggedHours,
            dto.SubtaskId,
            dto.Description,
            Status = "TimeLog Added"
        };

        return Ok(response);
    }
}
