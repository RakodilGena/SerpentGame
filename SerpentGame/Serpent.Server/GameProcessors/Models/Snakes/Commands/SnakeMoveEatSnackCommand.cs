using Serpent.Server.GameProcessors.Models.GameSettings;

namespace Serpent.Server.GameProcessors.Models.Snakes.Commands;

internal readonly struct SnakeMoveEatSnackCommand
{
    public FieldSettings FieldSettings { get; }
    public SnakeDirectionTypeDomain Direction { get; }

    public SnakeMoveEatSnackCommand(
        SnakeDirectionTypeDomain direction, 
        FieldSettings fieldSettings)
    {
        Direction = direction;
        FieldSettings = fieldSettings;
    }
}