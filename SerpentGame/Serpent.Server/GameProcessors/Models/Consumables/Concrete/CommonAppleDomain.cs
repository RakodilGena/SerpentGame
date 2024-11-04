using Serpent.Server.GameProcessors.Models.Consumables.Base;
using Serpent.Server.GameProcessors.Models.Consumables.Events;

namespace Serpent.Server.GameProcessors.Models.Consumables.Concrete;

internal sealed class CommonAppleDomain : ConsumableDomain
{
    public CommonAppleDomain(int x, int y, int reward) : base(x, y, reward)
    {
    }

    protected override ConsumableType GetConsumableType() => ConsumableType.CommonApple;
}