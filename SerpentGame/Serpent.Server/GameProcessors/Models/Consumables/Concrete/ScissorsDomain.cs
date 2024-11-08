using System.ComponentModel.DataAnnotations.Schema;
using Serpent.Server.GameProcessors.Models.Consumables.Base;

namespace Serpent.Server.GameProcessors.Models.Consumables.Concrete;

internal sealed class ScissorsDomain : ExpirableConsumableDomain
{
    [NotMapped] public int SegmentsToCut { get; }

    public ScissorsDomain(
        int x, int y, int ticksRemaining, int segmentsToCut)
        : base(x, y, ticksRemaining)
    {
        SegmentsToCut = segmentsToCut;
    }

    protected override ConsumableTypeDomain GetConsumableType() => ConsumableTypeDomain.Scissors;
}