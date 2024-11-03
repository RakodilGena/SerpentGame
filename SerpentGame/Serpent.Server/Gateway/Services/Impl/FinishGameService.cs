using Serpent.Server.Gateway.Models.Inner.FinishGameResults;

namespace Serpent.Server.Gateway.Services.Impl;

internal sealed class FinishGameService : IFinishGameService
{
    public Task<FinishGameResultDto> FinishGameAsync(Guid gameId)
    {
        return Task.FromResult(FinishGameResultDto.Success);
    }
}