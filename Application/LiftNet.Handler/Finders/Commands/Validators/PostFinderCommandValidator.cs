using FluentValidation;
using LiftNet.Handler.Finders.Commands.Requests;

public class PostFinderCommandValidator : AbstractValidator<CreateFinderPostCommand>
{
    public PostFinderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.StartTime)
            .LessThanOrEqualTo(x => x.EndTime)
            .WithMessage("StartTime must be less than or equal to EndTime.")
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("StartTime must not be earlier than the current UTC time.");

        RuleFor(x => x.StartPrice)
            .LessThanOrEqualTo(x => x.EndPrice)
            .WithMessage("StartPrice must be less than or equal to EndPrice.");
    }
}
