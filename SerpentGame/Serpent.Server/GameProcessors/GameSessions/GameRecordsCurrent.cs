namespace Serpent.Server.GameProcessors.GameSessions;

internal sealed class GameRecordsCurrent
{
    public int TotalScore { get; private set; }
    public int CommonApplesEaten { get; private set; }
    public int GoldenApplesEaten { get; private set; }
    public int ScissorsEaten { get; private set; }

    private const int CommonAppleScoreModifier = 10;
    private const int GoldenAppleScoreModifier = 50;
    private const int ScissorsScoreModifier = 20;

    public void OnCommonAppleEaten()
    {
        CommonApplesEaten++;
        TotalScore += CommonAppleScoreModifier;
    }

    public void OnGoldenAppleEaten()
    {
        GoldenApplesEaten++;
        TotalScore += GoldenAppleScoreModifier;
    }

    public void OnScissorsEaten()
    {
        ScissorsEaten++;
        TotalScore += ScissorsScoreModifier;
    }
}