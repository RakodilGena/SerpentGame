using Serpent.Server.Gateway.Models.Inner.UserRecords;

namespace Serpent.Server.Gateway.Services;

public interface IUserRecordsService
{
    public Task<IReadOnlyList<UserRecordDto>> GetUserRecordsAsync(CancellationToken cancellationToken);
}