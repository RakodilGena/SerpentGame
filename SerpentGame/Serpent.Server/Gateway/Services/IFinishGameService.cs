using Serpent.Server.Gateway.Models.Inner.FinishGameResults;

namespace Serpent.Server.Gateway.Services;

public interface IFinishGameService
{
    Task<FinishGameResultDto> FinishGameAsync(Guid gameId);
}