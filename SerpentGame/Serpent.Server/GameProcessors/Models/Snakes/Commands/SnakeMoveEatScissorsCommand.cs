using Serpent.Server.GameProcessors.Models.GameSettings;

namespace Serpent.Server.GameProcessors.Models.Snakes.Commands;

internal readonly struct SnakeMoveEatScissorsCommand
{
    public FieldSettings FieldSettings { get; }
    public SnakeDirectionTypeDomain Direction { get; }

    public int RequestedSegmentsToCut { get; }

    public SnakeMoveEatScissorsCommand(
        FieldSettings fieldSettings, 
        SnakeDirectionTypeDomain direction, 
        int requestedSegmentsToCut)
    {
        FieldSettings = fieldSettings;
        Direction = direction;
        RequestedSegmentsToCut = requestedSegmentsToCut;
    }
}