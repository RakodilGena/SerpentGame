namespace Serpent.Server.GameProcessors.Models.GameSettings;

internal readonly record struct GameSettings(
    Guid GameId,
    FieldSettings FieldSettings,
    TimeSpan GameSessionSleepTime,
    string Username);
