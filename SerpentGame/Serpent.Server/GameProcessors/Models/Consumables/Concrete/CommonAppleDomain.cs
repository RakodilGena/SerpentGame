using Serpent.Server.GameProcessors.Models.Consumables.Base;

namespace Serpent.Server.GameProcessors.Models.Consumables.Concrete;

internal sealed class CommonAppleDomain : ConsumableDomain
{
    public CommonAppleDomain(int x, int y) : base(x, y)
    {
    }

    protected override ConsumableType GetConsumableType() => ConsumableType.CommonApple;
}