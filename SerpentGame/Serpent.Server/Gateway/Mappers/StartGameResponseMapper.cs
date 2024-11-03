using System.Diagnostics;
using Serpent.Server.Gateway.Models.Inner.StartGameResults;
using Serpent.Server.Gateway.Models.Out.StartGameResponces;

namespace Serpent.Server.Gateway.Mappers;

internal static class StartGameResponseMapper
{
    public static StartGameResponse ToResponse(this StartGameResultDto dto)
    {
        Debug.Assert(dto.GameId is not null || dto.ErrorType is not null);

        if (dto.GameId is not null)
            return new StartGameResponse(dto.GameId.Value, StartGameResponseResult.Success);

        return new StartGameResponse(Guid.Empty, dto.ErrorType!.Value.ToResult());
    }

    public static StartGameResponseResult ToResult(this StartGameResultErrorsDto errors)
    {
        return errors switch
        {
            StartGameResultErrorsDto.TooManyGames => StartGameResponseResult.TooManyGames,

            _ => throw new ArgumentOutOfRangeException(
                nameof(errors),
                errors,
                "result was out of range of allowed values.")
        };
    }
}