﻿namespace Serpent.Server.Gateway.Models.Inner.StartGameRequests;

public sealed record StartGameRequestDto(
    string Username,
    FieldSizeDto FieldSize,
    bool WallsTransparent);