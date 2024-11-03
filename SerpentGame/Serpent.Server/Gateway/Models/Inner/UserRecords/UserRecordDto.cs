namespace Serpent.Server.Gateway.Models.Inner.UserRecords;

public sealed record UserRecordDto(
    string Username,
    int TotalScore);