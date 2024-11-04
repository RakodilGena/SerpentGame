using Serpent.Server.GameProcessors.Models.Consumables.Events;
using Serpent.Server.GameProcessors.Models.Consumables.Interfaces;
using Serpent.Server.GameProcessors.Models.Snakes.Events.ConsumeChecks;

namespace Serpent.Server.GameProcessors.Models.Consumables.Base;

internal abstract class ConsumableDomain
    : IConsumable
{
    public int X { get; }
    public int Y { get; }
    
    private readonly int _reward;

    protected ConsumableDomain(int x, int y, int reward)
    {
        X = x;
        Y = y;
        _reward = reward;
    }

    public event EventHandler<ConsumedEventArgs>? Consumed;

    public void OnConsumeCheck(ConsumeCheckEventArgs consumeCheckEventArgs)
    {
        if (consumeCheckEventArgs.X != X || consumeCheckEventArgs.Y != Y)
            return;

        consumeCheckEventArgs.ApplyConsumable(this);

        var consumedEventArgs = new ConsumedEventArgs(GetConsumableType(), _reward);
        Consumed?.Invoke(this, consumedEventArgs);
    }

    protected abstract ConsumableType GetConsumableType();
}