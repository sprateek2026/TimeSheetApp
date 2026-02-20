using FluentValidation;
using Timesheet.Api.DTOs.Timesheets;
using Timesheet.Api.Enums;

namespace Timesheet.Api.Validators
{
    public class CreateTimeLogDtoValidator : AbstractValidator<CreateTimesheetRequest>
    {
        public CreateTimeLogDtoValidator()
        {

            RuleFor(x => x.UserId)
                .GreaterThan(0);

            RuleFor(x => x.WorkDate)
                .NotEmpty();

            RuleFor(x => x.ApprovalStatus)
                .Must(s => s == TimesheetStatus.Draft || s == TimesheetStatus.Submitted)
                .WithMessage("Only Draft or Submitted allowed while creating timesheet");

            RuleFor(x => x.TimeLogs)
                .NotEmpty()
                .WithMessage("At least one time log is required");

            RuleForEach(x => x.TimeLogs)
                .SetValidator(new TimeLogItemValidator());

            RuleFor(x => x.TimeLogs.Sum(t => t.LoggedHours))
                .LessThanOrEqualTo(24)
                .WithMessage("Total logged hours cannot exceed 24");


            //RuleFor(x => x.SummaryId)
            //    .GreaterThan(0);

            //RuleFor(x => x.SubtaskId)
            //    .GreaterThan(0);

            //RuleFor(x => x.LoggedHours)
            //    .GreaterThan(0)
            //    .LessThanOrEqualTo(24);

            //RuleFor(x => x.TimeFrom)
            //    .LessThan(x => x.TimeTo)
            //    .WithMessage("TimeFrom must be earlier than TimeTo");

            //RuleFor(x => x.Description)
            //    .NotEmpty()
            //    .MaximumLength(500);
        }
        
    }
}
