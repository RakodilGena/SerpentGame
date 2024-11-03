using Serpent.Server.Gateway.Models.Out.StartGameResponces;

namespace Serpent.Server.Gateway.Models.Inner.StartGameResults;

public sealed class StartGameResultDto
{
    public Guid? GameId { get; }
    public StartGameResultErrorsDto? ErrorType { get; }

    private StartGameResultDto(Guid? gameId, StartGameResultErrorsDto? errorType)
    {
        GameId = gameId;
        ErrorType = errorType;
    }

    public static StartGameResultDto Success(Guid gameId)
    {
        return new StartGameResultDto(gameId, null);
    }

    public static StartGameResultDto Failure(StartGameResultErrorsDto? errorType)
    {
        return new StartGameResultDto(null, errorType);
    }
}