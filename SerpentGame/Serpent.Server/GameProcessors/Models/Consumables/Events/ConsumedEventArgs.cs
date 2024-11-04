namespace Serpent.Server.GameProcessors.Models.Consumables.Events;

internal sealed record ConsumedEventArgs(
    ConsumableType ConsumableType,
    int Reward);