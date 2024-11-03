namespace Serpent.Server.Gateway.Models.In.StartGameRequests;

public sealed record StartGameRequest(
    string? Username,
    FieldSize? FieldSize,
    bool? WallsTransparent);