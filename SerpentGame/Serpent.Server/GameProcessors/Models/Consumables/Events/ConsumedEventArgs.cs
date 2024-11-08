namespace Serpent.Server.GameProcessors.Models.Consumables.Events;

internal sealed record ConsumedEventArgs(
    ConsumableTypeDomain ConsumableType);