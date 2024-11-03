using Serpent.Server.Gateway.Models.Inner.StartGameRequests;
using Serpent.Server.Gateway.Models.Inner.StartGameResults;

namespace Serpent.Server.Gateway.Services.Impl;

internal sealed class StartGameService : IStartGameService
{
    public Task<StartGameResultDto> StartGameAsync(
        StartGameRequestDto request, 
        CancellationToken cancellationToken)
    {
        return Task.FromResult(StartGameResultDto.Success(Guid.NewGuid()));
    }
}