using Microsoft.EntityFrameworkCore;
using System;
using Timesheet.Api.Data;
using Timesheet.Api.DTOs.Login;
using Timesheet.Api.DTOs.Timesheets;
using Timesheet.Api.Entities;
using Timesheet.Api.Enums;
using Timesheet.Api.Models;
using TimeSheet.Api.Dtos.Timesheets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Timesheet.Api.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly TimesheetDbContext _context;


        public TimesheetService(TimesheetDbContext context)
        {
            _context = context;
        }
        private readonly HierarchyService _hierarchyService;


        public async Task<List<CustomerDTO>> GetCustomersAsync()
        {
            return await _context.TM_CUSTOMERS
                .OrderBy(x => x.ID)
                .Select(x => new CustomerDTO
                {
                    id = x.ID,
                    custcode = x.CUSTCODE,
                    isactive = x.ISACTIVE,
                    billingaddress = x.BILLINGADDRESS,  
                    description = x.DESCRIPTION
                })
                .ToListAsync();
        }

        public async Task<List<projectsDTO>> GetProjectsAsync(int custId)
        {
            return await _context.TM_PROJECTS
                 .Where(x => x.CUSTOMERID == custId)
                .OrderBy(x => x.ID)
                .Select(x => new projectsDTO
                {
                    id = x.ID,
                    projcode = x.PROJCODE,
                    customerid = x.CUSTOMERID,
                    description = x.DESCRIPTION,
                    isactive=x.ISACTIVE
                })
                .ToListAsync();
        }

        public async Task<List<phaseDTO>> GetPhasesAsync(int projId)
        {
            return await _context.TM_PHASE
                 .Where(x => x.PROJID == projId)
                .OrderBy(x => x.ID)
                .Select(x => new phaseDTO
                {
                    id = x.ID,
                    phasecode = x.PHASECODE,
                    isactive = x.ISACTIVE,
                    description = x.DESCRIPTION,
                    projid= x.PROJID
                })
                .ToListAsync();
        }

        public async Task<List<subphaseDTO>> GetSubPhasesAsync(int phaseId)
        {
            return await _context.TM_SUBPHASE
                 .Where(x => x.PHASEID == phaseId)
                .OrderBy(x => x.ID)
                .Select(x => new subphaseDTO
                {
                    id = x.ID,
                    subphasecode = x.SUBPHASECODE,
                    description = x.DESCRIPTION,
                    isactive = x.ISACTIVE,
                    phaseid= x.PHASEID
                })
                .ToListAsync();
        }
        public async Task<List<tasksDTO>> GetTasksAsync(int SubPhaseId)
        {
            return await _context.TM_TASKS
                 .Where(x => x.SUBPHASEID == SubPhaseId)
                .OrderBy(x => x.ID)
                .Select(x => new tasksDTO
                {
                    id = x.ID,
                    task = x.TASK,
                    isactive = x.ISACTIVE,
                    description = x.DESCRIPTION,
                    subphaseid= x.SUBPHASEID    
                })
                .ToListAsync();
        }

        public async Task<List<subtasksDTO>> GetSubTasksAsync(int taskId)
        {
            return await _context.TM_SUBTASKS
                 .Where(x => x.TASKID == taskId)
                .OrderBy(x => x.ID)
                .Select(x => new subtasksDTO
                {
                    id = x.ID,
                    subtask = x.SUBTASK,
                    isactive = x.ISACTIVE,
                    description = x.DESCRIPTION,   
                    taskid= x.TASKID    
                })
                .ToListAsync();
        }

        public async Task<TM_APPUSERS> ValidateLoginAsync(LoginRequest request)
        {
            var result = await _context.TM_APPUSERS
                //.FindAsync(request.SummaryId);
                .FirstOrDefaultAsync(x => x.EMAIL == request.email && x.PASSWORD_HASH == request.Password);
            if (result == null)
                return null;

            // Compare hashed password
            //if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            //    return null;

            return result;
        }

        //-----------------------------------


        public async Task<(List<SchedulerEventDto>, int)> GetSchedulerEventsAsync(
            //DateTime from, 
            //DateTime to, 
            int userId)
        {
            //var startDate = from.Date;
            //var endDate = to.Date;
            var query =
                from log in _context.TM_TIMELOGDETAILS
                join summary in _context.TM_DAILYSUMMARY
                    on log.SUMMARYID equals summary.ID
                where summary.USERID == userId
                //   && summary.WORKDATE >= startDate
                //&& summary.WORKDATE <= endDate
                select new SchedulerEventDto
                {
                    TimeLogId = log.ID,
                    Title = "Task " + log.SUBTASKID,
                    customerId= log.CUSTOMERID,
                    projectId= log.PROJECTID,
                    phaseId = log.PHASEID,
                    subPhaseId = log.SUBPHASEID,
                    taskId = log.TASKID,
                    SubTaskId = log.SUBTASKID,
                    Start = summary.WORKDATE.Add(log.TIMEFROM),
                    End = summary.WORKDATE.Add(log.TIMETO),
                    Description = log.DESCRIPTION,
                    Approvalstatus=log.APPROVALSTATUS,
                    IsAllDay = false,
                    SummaryId = summary.ID,                    
                };

            var data = await query.ToListAsync();
            return (data, data.Count);
        }

        public async Task SaveOrUpdateTimeLogAsync(SaveTimeLogRequestDTO req)
        {
            
            var localStart = req.Start.ToLocalTime();
            var localEnd = req.End.ToLocalTime();

            var workDate = localStart.Date;
            // 1️⃣ Find or create daily summary
            var summary = await _context.TM_DAILYSUMMARY
                .FirstOrDefaultAsync(x =>
                    x.USERID == req.UserId &&
                    x.WORKDATE == workDate);

            if (summary == null)
            {
                summary = new TM_DAILYSUMMARY
                {
                    USERID = req.UserId,
                    WORKDATE = workDate,
                    TOTALDAILYHR = 0
                };

                _context.TM_DAILYSUMMARY.Add(summary);
                await _context.SaveChangesAsync();
            }

            // 2️⃣ CREATE
            if (req.Id == null)
            {
                var entity = new TM_TIMELOGDETAILS
                {
                    SUMMARYID = summary.ID,
                    CUSTOMERID = req.CustomerId,
                    PROJECTID = req.ProjectId,
                    PHASEID = req.PhaseId,
                    SUBPHASEID = req.SubPhaseId,
                    TASKID = req.TaskId,
                    SUBTASKID = req.SubTaskId,
                    TIMEFROM = localStart.TimeOfDay,
                    TIMETO = localEnd.TimeOfDay,
                    LOGGEDHOURS = req.LoggedHours,
                    DESCRIPTION = req.Description,
                    APPROVALSTATUS = 1 // DRAFT
                };

                _context.TM_TIMELOGDETAILS.Add(entity);
            }
            else
            {
                // 3️⃣ UPDATE
                var entity = await _context.TM_TIMELOGDETAILS
                    .FirstOrDefaultAsync(x => x.ID == req.Id.Value);

                if (entity == null)
                    throw new Exception("Time entry not found");

                entity.CUSTOMERID = req.CustomerId;
                entity.PROJECTID = req.ProjectId;
                entity.PHASEID = req.PhaseId;
                entity.SUBPHASEID = req.SubPhaseId;
                entity.TASKID = req.TaskId;
                entity.SUBTASKID = req.SubTaskId;

                entity.TIMEFROM = localStart.TimeOfDay;
                entity.TIMETO = localEnd.TimeOfDay;
                entity.LOGGEDHOURS = req.LoggedHours;
                entity.DESCRIPTION = req.Description;
                entity.APPROVALSTATUS = 1;
                //_context.TM_TIMELOGDETAILS.Update(entity);
            }

            // 4️⃣ Recalculate daily total
            summary.TOTALDAILYHR = await _context.TM_TIMELOGDETAILS
                .Where(x => x.SUMMARYID == summary.ID)
                .SumAsync(x => x.LOGGEDHOURS);

            await _context.SaveChangesAsync();
        }
        public async Task DeleteTimeLogAsync(int id)
        {
            var entity = await _context.TM_TIMELOGDETAILS
                .FirstOrDefaultAsync(x => x.ID == id);

            if (entity == null)
                throw new Exception("Time log not found");

            _context.TM_TIMELOGDETAILS.Remove(entity);
            await _context.SaveChangesAsync();

            // 🔄 Update daily total
            var summary = await _context.TM_DAILYSUMMARY
                .FirstOrDefaultAsync(x => x.ID == entity.SUMMARYID);

            if (summary != null)
            {
                summary.TOTALDAILYHR = await _context.TM_TIMELOGDETAILS
                    .Where(x => x.SUMMARYID == summary.ID)
                    .SumAsync(x => x.LOGGEDHOURS);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TimesheetMonthDto>> GetMonthlyTimesheetAsync(int userId, int year, int month) 
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);

            var logs = await _context.TM_TIMELOGDETAILS
                .Where(x =>
                 //x.Summary.USERID == userId
                     x.Summary.User.MANAGERID == userId
                    //&&  x.Summary.WORKDATE >= from
                    //&&  x.Summary.WORKDATE < to
                    )
                .Select(x => new
                {
                    TimeLog = x,
                    Summary = x.Summary,
                    User = x.Summary.User,
                    SubTask = x.SubTask,
                    Task = x.SubTask.Task,
                    SubPhase = x.SubTask.Task.SubPhase,
                    Phase = x.SubTask.Task.SubPhase.Phase,
                    Project = x.SubTask.Task.SubPhase.Phase.Project,
                    Customer = x.SubTask.Task.SubPhase.Phase.Project.Customer
                })
                .ToListAsync();

            var result = logs
                .GroupBy(x => new
                {
                    TaskId = x.Task.ID,
                    SubTaskId = x.SubTask.ID,
                    UserId = x.Summary.USERID
                })
                .Select(g => new TimesheetMonthDto
                {
                    Id = g.Min(x => x.TimeLog.ID),             
                    EmployeeId = g.First().User.ID,
                    EmployeeName = g.First().User.FNAME + " " + g.First().User.LNAME,
                    ManagerId = g.First().User.MANAGERID==null? 1: g.First().User.MANAGERID,
                    UserId = g.Key.UserId,

                    CustomersId = g.First().Customer.ID,
                    CustomersName = g.First().Customer.CUSTCODE,
                    ProjectId = g.First().Project.ID,
                    ProjectName = g.First().Project.PROJCODE,
                    PhaseId = g.First().Phase.ID,
                    PhaseName = g.First().Phase.PHASECODE,
                    SubPhaseId = g.First().SubPhase.ID,
                    SubPhaseName = g.First().SubPhase.SUBPHASECODE,
                    TaskId = g.First().Task.ID,
                    TaskName = g.First().Task.TASK,
                    SubTaskId = g.First().SubTask.ID,
                    SubTaskName = g.First().SubTask.SUBTASK,

                    DailySummaries = g
                        .GroupBy(x => new
                        {
                            x.Summary.ID,
                            x.Summary.WORKDATE,
                            x.Summary.TOTALDAILYHR,
                            x.Summary.RAISEHANDFLAG,
                            x.Summary.INVOICEID
                        })
                        .Select(s => new DailySummaryDto
                        {
                            DailySummariesId = s.Key.ID,
                            UserId = g.Key.UserId,
                            SubTaskId = g.First().SubTask.ID,
                            WorkDate = s.Key.WORKDATE.ToShortDateString(),
                            TotalDailyHr = s.Key.TOTALDAILYHR,
                            RaiseHandFlag = false,//s.Key.RAISEHANDFLAG ?? false,
                            InvoiceId = 1,//s.Key.INVOICEID
                        })
                        .OrderBy(d => d.WorkDate)
                        .ToList(),

                    TimeLogDetails = g
                        .Select(x => new TimeLogDetailDto
                        {
                            TimeLogDetailsId = x.TimeLog.ID,
                            SummaryId = x.TimeLog.SUMMARYID,
                            TimeFrom = x.TimeLog.TIMEFROM,
                            TimeTo = x.TimeLog.TIMETO,
                            LoggedHr = x.TimeLog.LOGGEDHOURS,
                            SubTaskId = x.TimeLog.SUBTASKID,
                            Description = x.TimeLog.DESCRIPTION,
                            ApprovalStatus = x.TimeLog.APPROVALSTATUS,
                            ApprovedBy= 1,
                            ApprovedDate= "2024-01-02 17:30:00"
                        })
                        .OrderBy(t => t.SummaryId)
                        .ThenBy(t => t.TimeFrom)
                        .ToList()
                })
                .ToList();

            return result;
        }
        public async Task ActionOnTimeLogAsync(TimesheetActionRequest request)
        {
            var summary = await _context.TM_TIMELOGDETAILS
                //.FindAsync(request.SummaryId);
                .FirstOrDefaultAsync(x => x.SUMMARYID == request.SummaryId && x.ID == request.Id);
            if (summary == null)
                throw new Exception("Timesheet not found");


            summary.APPROVALSTATUS = request.Action;
            summary.APPROVEDBY = request.ActionByUserId;
            summary.APPROVALDATE = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.Comment))
            {
                _context.TM_COMMENTS.Add(new TM_COMMENTS
                {
                    SUMMARYID = summary.ID,
                    COMMENTTEXT = request.Comment,
                    COMMENTBY = request.ActionByUserId
                });
            }

            await _context.SaveChangesAsync();
        }

        
        //----------------------------------------------end used code------------------------------------------------//
        public async Task<List<TM_DAILYSUMMARY>> GetDailySummuryByUserAsync(int userId)
        {
            return await _context.TM_DAILYSUMMARY
                .Where(x => x.USERID == userId)
                .OrderByDescending(x => x.WORKDATE)
                .ToListAsync();
        }
        public async Task<int> CreateTimesheetAsync(CreateTimesheetRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var totalHours = request.TimeLogs.Sum(x => x.LoggedHours);

                // Use FIRST subtask as primary reference
                var mainSubtaskId = request.TimeLogs.First().SubtaskId;

                var summary = new TM_DAILYSUMMARY
                {
                    USERID = request.UserId,
                    WORKDATE = request.WorkDate,
                    //SUBTASKID = mainSubtaskId,
                    TOTALDAILYHR = totalHours,
                    //APPROVALSTATUS = request.ApprovalStatus
                };

                _context.TM_DAILYSUMMARY.Add(summary);
                await _context.SaveChangesAsync();

                var timeLogs = request.TimeLogs.Select(t => new TM_TIMELOGDETAILS
                {
                    SUMMARYID = summary.ID,
                    TIMEFROM = t.TimeFrom,
                    TIMETO = t.TimeTo,
                    LOGGEDHOURS = t.LoggedHours,
                    SUBTASKID = t.SubtaskId,
                    DESCRIPTION = t.Description
                });

                _context.TM_TIMELOGDETAILS.AddRange(timeLogs);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return summary.ID;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }      
        public async Task<TM_DAILYSUMMARY> AddAsync(TM_DAILYSUMMARY summary)
        {
            _context.TM_DAILYSUMMARY.Add(summary);
            await _context.SaveChangesAsync();
            return summary;
        }

        public async Task<int> CreateDraftAsync(CreateTimesheetDto dto)
        {
            var entity = new TM_DAILYSUMMARY
            {
                USERID = dto.UserId,
                //SUBTASKID = dto.SubTaskId,
                WORKDATE = dto.WorkDate,
                TOTALDAILYHR = dto.TotalDailyHr,
                //APPROVALSTATUS = TimesheetStatus.Draft
            };

            _context.TM_DAILYSUMMARY.Add(entity);
            await _context.SaveChangesAsync();

            var detail = new TM_TIMELOGDETAILS
            {
                SUMMARYID = entity.ID,
                TIMEFROM = dto.TimeFrom,
                TIMETO = dto.TimeTo,
                LOGGEDHOURS = dto.TotalDailyHr,
                SUBTASKID = dto.SubTaskId,
                DESCRIPTION = dto.Description
            };
            _context.TM_TIMELOGDETAILS.Add(detail);
            await _context.SaveChangesAsync();
            return entity.ID;
        }
        public async Task SubmitAsync(int timesheetId, int userId)
        {
            var ts = await GetTimesheet(timesheetId);

            if (ts.USERID != userId)
                throw new Exception("You can submit only your own timesheet.");

            //if (ts.APPROVALSTATUS != TimesheetStatus.Draft)
            //    throw new Exception("Only Draft can be submitted.");

            //ts.APPROVALSTATUS = TimesheetStatus.Submitted;
            //await _context.SaveChangesAsync();
        }
        public async Task ApproveAsync(int timesheetId, int managerId)
        {
            var ts = await GetTimesheet(timesheetId);

            var employee = await _context.AppUsers.FindAsync(ts.USERID);
            if (employee.ManagerId != managerId)
                throw new Exception("You are not authorized to approve.");

            //if (ts.ApprovalStatus != TimesheetStatus.Submitted)
            //    throw new Exception("Only Submitted can be approved.");

            //ts.ApprovalStatus = TimesheetStatus.Approved;
            await _context.SaveChangesAsync();
        }
        public async Task RejectAsync(int timesheetId, int managerId, string reason)
        {
            var ts = await GetTimesheet(timesheetId);

            var employee = await _context.AppUsers.FindAsync(ts.USERID);
            if (employee.ManagerId != managerId)
                throw new Exception("You are not authorized to reject.");

            //ts.ApprovalStatus = TimesheetStatus.Rejected;
            await _context.SaveChangesAsync();
        }

        public async Task SubmitTimesheetAsync(int summaryId)
        {
            var summary = await _context.TM_DAILYSUMMARY.FindAsync(summaryId);
            if (summary == null)
                throw new Exception("Timesheet not found");

            //if (summary.ApprovalStatus != TimesheetStatus.Draft)
            //    throw new Exception("Only Draft can be submitted");
            //summary.ApprovalStatus = TimesheetStatus.Submitted;

            await _context.SaveChangesAsync();
        }
        
        public async Task RejectTimesheetAsync(TimesheetActionRequest request)
        {
            var summary = await _context.TM_DAILYSUMMARY.FindAsync(request.SummaryId);
            if (summary == null)
                throw new Exception("Timesheet not found");

            //summary.ApprovalStatus = TimesheetStatus.Rejected;
            //summary.ApprovedBy = request.ActionByUserId;
            //summary.ApprovalDate = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.Comment))
            {
                _context.TM_COMMENTS.Add(new TM_COMMENTS
                {
                    SUMMARYID = summary.ID,
                    COMMENTTEXT = request.Comment,
                    COMMENTBY = request.ActionByUserId
                });
            }

            await _context.SaveChangesAsync();
        }
        private async Task<TM_DAILYSUMMARY> GetTimesheet(int id)
        {
            var ts = await _context.TM_DAILYSUMMARY.FindAsync(id);
            if (ts == null)
                throw new Exception("Timesheet not found.");
            return ts;
        }

        public async Task<List<TimesheetResponseDto>> GetTimesheetsByUserAsync(int userId)
        {
            return await _context.TM_DAILYSUMMARY
                .Where(x => x.USERID == userId)
                .Include(x => x.TOTALDAILYHR)
                .OrderByDescending(x => x.WORKDATE)
                .Select(x => new TimesheetResponseDto
                {
                    SummaryId = x.ID,
                    UserId = x.USERID,
                    WorkDate = x.WORKDATE,
                    TotalHours = x.TOTALDAILYHR,
                    //ApprovalStatus = x.APPROVALSTATUS
                    //TimeLogs = x.TmTimeLogDetail.Select(t => new TimeLogDto
                    //{
                    //    TimeFrom = t.TimeFrom,
                    //    TimeTo = t.TimeTo,
                    //    LoggedHours = t.LoggedHours,
                    //    SubtaskId = t.SubtaskId,
                    //    Description = t.Description
                    //}).ToList()
                })
                .ToListAsync();
        }
        public async Task<List<TimesheetResponseDto>> GetTimesheetsByDateRangeAsync(
            DateTime fromDate, DateTime toDate)
        {
            return await _context.TM_DAILYSUMMARY
                .Where(x => x.WORKDATE >= fromDate && x.WORKDATE <= toDate)
                .Include(x => x.TOTALDAILYHR)
                .Select(x => new TimesheetResponseDto
                {
                    SummaryId = x.ID,
                    UserId = x.USERID,
                    WorkDate = x.WORKDATE,
                    TotalHours = x.TOTALDAILYHR,
                    //ApprovalStatus = (int)x.APPROVALSTATUS,
                    //TimeLogs = x.TimeLogs.Select(t => new TimeLogDto
                    //{
                    //    TimeFrom = t.TimeFrom,
                    //    TimeTo = t.TimeTo,
                    //    LoggedHours = t.LoggedHours,
                    //    SubtaskId = t.SubtaskId,
                    //    Description = t.Description
                    //}).ToList()
                })
                .ToListAsync();
        }
        //public async Task<TimesheetResponseDto?> GetTimesheetByIdAsync(int summaryId)
        //{
        //    return await _context.TM_DAILYSUMMARY
        //        .Where(x => x.ID == summaryId)
        //        //.Include(x => x.TimeLogs)
        //        .Select(x => new SchedulerEventDto
        //        {
        //            SummaryId = x.ID,
        //            UserId = x.USERID,
        //            WorkDate = x.WORKDATE,
        //            TotalHours = x.TOTALDAILYHR,
        //            ApprovalStatus = (int)x.APPROVALSTATUS,
        //            //TimeLogs = x.TimeLogs.Select(t => new TimeLogDto
        //            //{
        //            //    TimeFrom = t.TimeFrom,
        //            //    TimeTo = t.TimeTo,
        //            //    LoggedHours = t.LoggedHours,
        //            //    SubtaskId = t.SubtaskId,
        //            //    Description = t.Description
        //            //}).ToList()
        //        })
        //        .FirstOrDefaultAsync();
        //}
        

        public async Task<List<ManagerTimesheetDto>> GetPendingForManagerAsync(int managerId)
        {
            var employeeIds = await _hierarchyService.GetAllSubordinatesAsync(managerId);

            return await _context.TM_DAILYSUMMARY
                .Where(t =>
                    employeeIds.Contains(t.USERID))
                   //&& (t.APPROVALSTATUS == TimesheetStatus.Submitted ||
                   //  t.APPROVALSTATUS == TimesheetStatus.RaisedHand))
                .Select(t => new ManagerTimesheetDto
                {
                    SummaryId = t.ID,
                    EmployeeId = t.USERID,
                    EmployeeName = t.User.FNAME + " " + t.User.LNAME,
                    WorkDate = t.WORKDATE,
                    TotalHours = t.TOTALDAILYHR,
                    //Status = (int)t.APPROVALSTATUS
                })
                .OrderBy(t => t.WorkDate)
                .ToListAsync();
        }
    }
}
