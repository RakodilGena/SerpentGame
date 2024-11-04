using Serpent.Server.GameProcessors.Models.GameSettings;

namespace Serpent.Server.GameProcessors.Models.Snakes.Commands;

internal readonly struct SnakeMoveNoEatCommand
{
    public FieldSettings FieldSettings { get; }
    public SnakeDirectionTypeDomain Direction { get; }

    public SnakeMoveNoEatCommand(
        SnakeDirectionTypeDomain direction, 
        FieldSettings fieldSettings)
    {
        Direction = direction;
        FieldSettings = fieldSettings;
    }
}