﻿namespace Serpent.Server.GameProcessors.Models.Consumables.Events;

internal sealed record ExpiredEventArgs(
    ConsumableTypeDomain ConsumableType);