namespace Serpent.Server.GameProcessors.Models.Consumables.Interfaces;

internal interface IExpirable
{
    event EventHandler Expire;
    
    void OnTick(object? sender, EventArgs e);
}