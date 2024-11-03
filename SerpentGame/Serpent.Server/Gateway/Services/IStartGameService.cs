using Serpent.Server.Gateway.Models.Inner.StartGameRequests;
using Serpent.Server.Gateway.Models.Inner.StartGameResults;

namespace Serpent.Server.Gateway.Services;

public interface IStartGameService
{
    Task<StartGameResultDto> StartGameAsync(
        StartGameRequestDto request,
        CancellationToken cancellationToken);
}