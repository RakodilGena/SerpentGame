namespace Serpent.Server.GameProcessors.GameSessions;

internal sealed class GameSession
{
    private UserInputDomain _lastInput = UserInputDomain.None;
    private readonly GameSessionSleepTime _sleepTime;
    private readonly bool _wallsTransparent;

    private async Task GameCycleAsync()
    {
        
    }

    public void A()
    {
        int[] arr = [1, 2, 3, 4];
        for (int i = 0; i < arr.Length; i++)
        {
            ref int elem = ref arr[i];
            elem += 1;
        }
    }
}