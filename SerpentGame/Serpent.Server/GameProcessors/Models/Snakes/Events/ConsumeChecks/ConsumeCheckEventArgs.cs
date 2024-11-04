using Serpent.Server.GameProcessors.Models.Consumables.Base;

namespace Serpent.Server.GameProcessors.Models.Snakes.Events.ConsumeChecks;

internal sealed class ConsumeCheckEventArgs
{
    public ConsumableDomain? CollidedConsumable { get; private set; }
    
    public int X { get; }
    public int Y { get; }
    
    public bool Processed { get; private set; }

    public ConsumeCheckEventArgs(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void ApplyConsumable(ConsumableDomain collidedConsumable)
    {
        CollidedConsumable = collidedConsumable;
        Processed = true;
    }
}