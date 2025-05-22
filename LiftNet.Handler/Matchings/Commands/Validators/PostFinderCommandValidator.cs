using FluentValidation;
using LiftNet.Handler.Matchings.Commands.Requests;

public class PostFinderCommandValidator : AbstractValidator<PostFinderCommand>
{
    public PostFinderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.StartTime)
            .LessThanOrEqualTo(x => x.EndTime)
            .WithMessage("StartTime must be less than or equal to EndTime.");

        RuleFor(x => x.StartPrice)
            .LessThanOrEqualTo(x => x.EndPrice)
            .WithMessage("StartPrice must be less than or equal to EndPrice.");
    }
}
