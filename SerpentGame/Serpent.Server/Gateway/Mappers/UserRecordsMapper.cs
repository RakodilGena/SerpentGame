using Serpent.Server.Gateway.Models.Inner.UserRecords;
using Serpent.Server.Gateway.Models.Out.UserRecordsResponses;

namespace Serpent.Server.Gateway.Mappers;

internal static class UserRecordsMapper
{
    public static UserRecordsResponse ToResponse(this IReadOnlyList<UserRecordDto> userRecords)
    {
        if (userRecords.Count is 0)
            return new UserRecordsResponse([]);

        return new UserRecordsResponse(
            userRecords.Select(x => x.ToResponse()).ToArray());
    }
    
    private static UserRecord ToResponse(this UserRecordDto userRecord)
    {
        return new UserRecord(userRecord.Username, userRecord.TotalScore);
    }
}