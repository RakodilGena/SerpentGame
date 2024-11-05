using Serpent.Server.GameProcessors.Models.Consumables.Events;
using Serpent.Server.GameProcessors.Models.Consumables.Interfaces;
using Serpent.Server.GameProcessors.Models.Snakes.Events.ConsumeChecks;

namespace Serpent.Server.GameProcessors.Models.Consumables.Base;

internal abstract class ConsumableDomain
    : IConsumable
{
    public int X { get; }
    public int Y { get; }

    protected ConsumableDomain(int x, int y)
    {
        X = x;
        Y = y;
    }

    public event EventHandler<ConsumedEventArgs>? Consumed;

    public void OnConsumeCheck(ConsumeCheckEventArgs consumeCheckEventArgs)
    {
        //skip check if something already eaten
        if (consumeCheckEventArgs.Processed || consumeCheckEventArgs.X != X || consumeCheckEventArgs.Y != Y)
            return;

        consumeCheckEventArgs.ApplyConsumable(this);

        var consumedEventArgs = new ConsumedEventArgs(GetConsumableType());
        Consumed?.Invoke(this, consumedEventArgs);
    }

    protected abstract ConsumableType GetConsumableType();
}