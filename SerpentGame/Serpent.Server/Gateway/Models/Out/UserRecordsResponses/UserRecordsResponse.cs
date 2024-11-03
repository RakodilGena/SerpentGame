namespace Serpent.Server.Gateway.Models.Out.UserRecordsResponses;

public sealed record UserRecordsResponse(
    IReadOnlyList<UserRecord> UserRecords);