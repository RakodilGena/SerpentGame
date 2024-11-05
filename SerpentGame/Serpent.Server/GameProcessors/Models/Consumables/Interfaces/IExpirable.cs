using Serpent.Server.GameProcessors.Models.Consumables.Events;

namespace Serpent.Server.GameProcessors.Models.Consumables.Interfaces;

internal interface IExpirable
{
    event EventHandler<ExpiredEventArgs> Expire;
    
    void OnTick(object? sender, EventArgs e);
}