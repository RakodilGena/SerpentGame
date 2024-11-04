using Serpent.Server.GameProcessors.Models.Consumables.Events;
using Serpent.Server.GameProcessors.Models.Snakes.Events.ConsumeChecks;

namespace Serpent.Server.GameProcessors.Models.Consumables.Interfaces;

internal interface IConsumable
{
    event EventHandler<ConsumedEventArgs>? Consumed;

    void OnConsumeCheck(ConsumeCheckEventArgs consumeCheckEventArgs);
}