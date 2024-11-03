using FluentValidation;
using Serpent.Server.Gateway.Models.In.FinishGameRequests;

namespace Serpent.Server.Gateway.Validators;

internal sealed class FinishGameRequestValidator : AbstractValidator<FinishGameRequest>
{
    public FinishGameRequestValidator()
    {
        RuleFor(x => x.GameId)
            .NotNull()
            .WithMessage("Please specify a valid game ID.")
            .DependentRules(() =>
            {
                RuleFor(x => x.GameId).NotEmpty()
                    .WithMessage("Please specify a valid game ID.");
            });
    }
}