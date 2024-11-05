using System.Diagnostics;
using Serpent.Server.GameProcessors.Models.Consumables.Events;
using Serpent.Server.GameProcessors.Models.Consumables.Interfaces;

namespace Serpent.Server.GameProcessors.Models.Consumables.Base;

internal abstract class ExpirableConsumableDomain : ConsumableDomain, IExpirable
{
    private int _ticksRemaining;

    protected ExpirableConsumableDomain(
        int x,
        int y,
        int ticksRemaining)
        : base(x, y)
    {
        Debug.Assert(ticksRemaining > 0);

        _ticksRemaining = ticksRemaining;
    }

    public event EventHandler<ExpiredEventArgs>? Expire;

    public void OnTick(object? sender, EventArgs e)
    {
        if (_ticksRemaining is 0)
        {
            Expire?.Invoke(this, new ExpiredEventArgs(GetConsumableType()));
            return;
        }

        _ticksRemaining--;
    }
}