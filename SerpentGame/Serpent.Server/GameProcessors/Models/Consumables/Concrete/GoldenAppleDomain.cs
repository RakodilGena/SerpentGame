using Serpent.Server.GameProcessors.Models.Consumables.Base;
using Serpent.Server.GameProcessors.Models.Consumables.Events;

namespace Serpent.Server.GameProcessors.Models.Consumables.Concrete;

internal sealed class GoldenAppleDomain : ExpirableConsumableDomain
{
    public GoldenAppleDomain(int x, int y, int reward, int ticksRemaining) 
        : base(x, y, reward, ticksRemaining)
    {
    }

    protected override ConsumableType GetConsumableType() => ConsumableType.GoldenApple;
}
