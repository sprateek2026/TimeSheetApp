using FluentValidation;
using Timesheet.Api.DTOs.Timesheets;

namespace Timesheet.Api.Validators
{
    public class TimeLogItemValidator : AbstractValidator<TimeLogItemDto>
    {
        public TimeLogItemValidator()
        {
            RuleFor(x => x.SubtaskId)
                .GreaterThan(0);

            RuleFor(x => x.TimeFrom)
                .LessThan(x => x.TimeTo);

            RuleFor(x => x.LoggedHours)
                .GreaterThan(0);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}
