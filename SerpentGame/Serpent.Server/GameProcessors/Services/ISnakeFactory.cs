using Serpent.Server.GameProcessors.Models.Snakes;

namespace Serpent.Server.GameProcessors.Services;

internal interface ISnakeFactory
{
    SnakeDomain CreateSnakeDirectionUp(int maxPosition);
}