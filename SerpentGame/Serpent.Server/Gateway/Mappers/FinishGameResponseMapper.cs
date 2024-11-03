using Serpent.Server.Gateway.Models.Inner.FinishGameResults;
using Serpent.Server.Gateway.Models.Out.FinishGameResponses;

namespace Serpent.Server.Gateway.Mappers;

internal static class FinishGameResponseMapper
{
    public static FinishGameResponse ToResponse(this FinishGameResultDto dto)
    {
        var result = dto switch
        {
            FinishGameResultDto.Success => FinishGameResult.Success,
            FinishGameResultDto.NoSuchGame => FinishGameResult.NoSuchGame,

            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, "result was out of range of allowed values")
        };

        return new FinishGameResponse(result);
    }
}