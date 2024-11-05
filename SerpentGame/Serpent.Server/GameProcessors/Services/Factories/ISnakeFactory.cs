using Serpent.Server.GameProcessors.Models.Snakes;

namespace Serpent.Server.GameProcessors.Services.Factories;

internal interface ISnakeFactory
{
    SnakeDomain CreateSnakeDirectionUp(int maxPosition);
}