using Serpent.Server.GameProcessors.Models.Consumables.Concrete;
using Serpent.Server.GameProcessors.Models.GameSettings;

namespace Serpent.Server.GameProcessors.Services.Factories.Impl;

internal sealed class ConsumablesFactory : IConsumablesFactory
{
    private int _maxPosition;

    public void Initialize(FieldSettings settings)
    {
        _maxPosition = settings.MaxPosition;
    }

    public CommonAppleDomain CreateCommonApple(int x, int y)
    {
        return new CommonAppleDomain(x, y);
    }

    public GoldenAppleDomain CreateGoldenApple(int x, int y)
    {
        var ticks = (int)(_maxPosition * 1.5);

        return new GoldenAppleDomain(x, y, ticks);
    }

    public ScissorsDomain CreateScissors(int x, int y)
    {
        var ticks = _maxPosition * 3;
        const int segmentsToCut = 2;

        return new ScissorsDomain(x, y, ticks, segmentsToCut);
    }
}