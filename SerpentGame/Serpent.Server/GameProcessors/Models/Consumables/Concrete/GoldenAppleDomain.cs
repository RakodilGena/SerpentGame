using Serpent.Server.GameProcessors.Models.Consumables.Base;

namespace Serpent.Server.GameProcessors.Models.Consumables.Concrete;

internal sealed class GoldenAppleDomain : ExpirableConsumableDomain
{
    public GoldenAppleDomain(int x, int y, int ticksRemaining)
        : base(x, y, ticksRemaining)
    {
    }

    protected override ConsumableType GetConsumableType() => ConsumableType.GoldenApple;
}
