using Serpent.Server.GameProcessors.Models.Consumables.Concrete;
using Serpent.Server.GameProcessors.Models.GameSettings;

namespace Serpent.Server.GameProcessors.Services.Factories;

internal interface IConsumablesFactory
{
    void Initialize(FieldSettings settings);
    
    CommonAppleDomain CreateCommonApple(int x, int y);
    
    GoldenAppleDomain CreateGoldenApple(int x, int y);
    
    ScissorsDomain CreateScissors(int x, int y);
}