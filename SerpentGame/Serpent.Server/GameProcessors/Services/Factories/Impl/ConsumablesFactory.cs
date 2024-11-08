using System.Diagnostics;
using Serpent.Server.GameProcessors.Models.Consumables.Concrete;
using Serpent.Server.GameProcessors.Models.GameSettings;

namespace Serpent.Server.GameProcessors.Services.Factories.Impl;

internal sealed class ConsumablesFactory : IConsumablesFactory
{
    private bool _initialized;
    
    private int _fieldSizeScaleFactor;

    public void Initialize(FieldSettings settings)
    {
        _fieldSizeScaleFactor = settings.MaxPosition;
        _initialized = true;
    }

    public CommonAppleDomain CreateCommonApple(int x, int y)
    {
        Debug.Assert(_initialized);
        return new CommonAppleDomain(x, y);
    }

    public GoldenAppleDomain CreateGoldenApple(int x, int y)
    {
        Debug.Assert(_initialized);
        var ticks = (int)(_fieldSizeScaleFactor * 1.5);

        return new GoldenAppleDomain(x, y, ticks);
    }

    public ScissorsDomain CreateScissors(int x, int y)
    {
        Debug.Assert(_initialized);
        var ticks = _fieldSizeScaleFactor * 3;
        const int segmentsToCut = 2;

        return new ScissorsDomain(x, y, ticks, segmentsToCut);
    }
}