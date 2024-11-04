namespace Serpent.Server.GameProcessors.Models.GameSettings;

/// <summary>
/// 5-8 bytes.
/// </summary>
internal readonly struct FieldSettings
{
    public int MaxPosition { get; }
    public bool WallsTransparent { get; }

    public FieldSettings(int maxPosition, bool wallsTransparent)
    {
        MaxPosition = maxPosition;
        WallsTransparent = wallsTransparent;
    }
}