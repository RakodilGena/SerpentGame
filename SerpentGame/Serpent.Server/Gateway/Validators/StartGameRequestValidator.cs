using FluentValidation;
using Serpent.Server.Gateway.Models.In.StartGameRequests;

namespace Serpent.Server.Gateway.Validators;

internal sealed class StartGameRequestValidator : AbstractValidator<StartGameRequest>
{
    //todo add profanity validation
    public StartGameRequestValidator()
    {
        RuleFor(x => x.Username)
            .Length(5, 15).WithMessage("Please specify username between 5 and 15 characters");

        RuleFor(x => x.FieldSize)
            .IsInEnum()
            .WithMessage("Please specify correct field size");

        RuleFor(x => x.WallsTransparent)
            .NotNull()
            .WithMessage("Please specify walls transparency");
    }
}