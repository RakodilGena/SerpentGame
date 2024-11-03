using Serpent.Server.Gateway.Models.Inner.UserRecords;

namespace Serpent.Server.Gateway.Services.Impl;

internal sealed class UserRecordsService : IUserRecordsService
{
    public Task<IReadOnlyList<UserRecordDto>> GetUserRecordsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<UserRecordDto>>([]);
    }
}