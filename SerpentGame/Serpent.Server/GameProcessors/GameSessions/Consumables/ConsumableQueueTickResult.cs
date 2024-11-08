namespace Serpent.Server.GameProcessors.GameSessions.Consumables;

internal readonly record struct ConsumableQueueTickResult(
    bool CreateCommonApple,
    bool CreateGoldenApple,
    bool CreateScissors);