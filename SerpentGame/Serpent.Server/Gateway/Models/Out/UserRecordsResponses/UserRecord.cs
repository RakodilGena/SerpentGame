namespace Serpent.Server.Gateway.Models.Out.UserRecordsResponses;

public sealed record UserRecord(
    string Username,
    int TotalScore);