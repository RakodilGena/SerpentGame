using System.ComponentModel.DataAnnotations.Schema;
using Serpent.Server.GameProcessors.Models.Consumables.Base;
using Serpent.Server.GameProcessors.Models.Consumables.Events;

namespace Serpent.Server.GameProcessors.Models.Consumables.Concrete;

internal sealed class ScissorsDomain : ExpirableConsumableDomain
{
    [NotMapped] public int SegmentsToCut { get; }

    public ScissorsDomain(
        int x, int y, int reward, int ticksRemaining, int segmentsToCut)
        : base(x, y, reward, ticksRemaining)
    {
        SegmentsToCut = segmentsToCut;
    }

    protected override ConsumableType GetConsumableType() => ConsumableType.Scissors;
}