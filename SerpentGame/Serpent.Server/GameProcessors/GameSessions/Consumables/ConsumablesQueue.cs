namespace Serpent.Server.GameProcessors.GameSessions.Consumables;

internal sealed class ConsumablesQueue
{
    private sbyte _commonAppleTicks;
    private sbyte _goldenAppleTicks;
    private sbyte _scissorsTicks;

    private const byte GoldenAppleTicksLower = 40;
    private const byte GoldenAppleTicksUpper = 60;
    
    private const byte ScissorsTicksLower = 58;
    private const byte ScissorsTicksUpper = 82;

    public ConsumablesQueue(bool enqueueAllConsumables)
    {
        if (enqueueAllConsumables)
        {
            EnqueueCommonApple();
            EnqueueGoldenApple();
            EnqueueScissors();
        }
    }
    
    public ConsumableQueueTickResult Tick()
    {
        var commonAppleResult = TickQueue(ref _commonAppleTicks);
        var goldenAppleResult = TickQueue(ref _goldenAppleTicks);
        var scissorsResult = TickQueue(ref _scissorsTicks);
        
        return new ConsumableQueueTickResult(commonAppleResult, goldenAppleResult, scissorsResult);
    }

    private static bool TickQueue(ref sbyte ticks)
    {
        switch (ticks)
        {
            //inactive
            case -1:
                return false;
            
            //active, make inactive and return positive result.
            case 0:
                ticks--;
                return true;
            
            //active, not ready
            default:
                ticks--;
                return false;
        }
    }

    public void EnqueueCommonApple()
    {
        if (_commonAppleTicks is not -1)
            return;

        _commonAppleTicks = 0;
    }

    public void EnqueueGoldenApple()
    {
        if (_goldenAppleTicks is not -1)
            return;
        
        var ticks = (sbyte)Random.Shared.Next(GoldenAppleTicksLower, GoldenAppleTicksUpper);

        _goldenAppleTicks = ticks;
    }

    public void EnqueueScissors()
    {
        if (_scissorsTicks is not -1)
            return;
        
        var ticks = (sbyte)Random.Shared.Next(ScissorsTicksLower, ScissorsTicksUpper);

        _scissorsTicks = ticks;
    }
}