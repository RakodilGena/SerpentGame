namespace Serpent.Server.Gateway.Models.Out.StartGameResponces;

public sealed record StartGameResponse(
    Guid GameId,
    StartGameResponseResult ResultType);