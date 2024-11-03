namespace Serpent.Server.Gateway.Models.In.FinishGameRequests;

public sealed record FinishGameRequest(
    Guid? GameId);