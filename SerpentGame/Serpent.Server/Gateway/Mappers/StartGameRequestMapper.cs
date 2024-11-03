using FluentValidation;
using Serpent.Server.Gateway.Models.In.StartGameRequests;
using Serpent.Server.Gateway.Models.Inner.StartGameRequests;
using Serpent.Server.Gateway.Validators;

namespace Serpent.Server.Gateway.Mappers;

internal static class StartGameRequestMapper
{
    public static StartGameRequestDto ToDto(
        this StartGameRequest request)
    {
        var validator = new StartGameRequestValidator();
        validator.ValidateAndThrow(request);

        return new StartGameRequestDto(
            request.Username!,
            request.FieldSize.ToDto(),
            request.WallsTransparent ?? false);
    }

    private static FieldSizeDto ToDto(this FieldSize? fieldSize)
    {
        return fieldSize switch
        {
            FieldSize.Medium => FieldSizeDto.Medium,
            FieldSize.Large => FieldSizeDto.Large,

            _ => FieldSizeDto.Small
        };
    }
}