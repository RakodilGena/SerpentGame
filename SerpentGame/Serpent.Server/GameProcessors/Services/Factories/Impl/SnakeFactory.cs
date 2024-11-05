using System.Diagnostics;
using Serpent.Server.GameProcessors.Models.Snakes;
using Serpent.Server.GameProcessors.Models.Snakes.Segments;
using Serpent.Server.GameProcessors.Models.Snakes.Segments.Directions;

namespace Serpent.Server.GameProcessors.Services.Factories.Impl;

internal sealed class SnakeFactory : ISnakeFactory
{
    public SnakeDomain CreateSnakeDirectionUp(int maxPosition)
    {
        var middlePoint = GetMiddlePoint(maxPosition);

        var head = new SnakeHeadSegmentDomain(middlePoint, middlePoint - 2, SnakeDirectionTypeDomain.Up);
        List<SnakeBodySegmentDomain> body =
        [
            new(x: middlePoint, y: middlePoint - 1, SnakeBodySegmentDirectionTypeDomain.Up),
            new(x: middlePoint, y: middlePoint, SnakeBodySegmentDirectionTypeDomain.Up),
            new(x: middlePoint, y: middlePoint + 1, SnakeBodySegmentDirectionTypeDomain.Up)
        ];
        var tail = new SnakeTailSegmentDomain(middlePoint, middlePoint + 2, SnakeDirectionTypeDomain.Up);

        return new SnakeDomain(head, body, tail);
    }

    private int GetMiddlePoint(int maxPosition)
    {
        Debug.Assert(maxPosition % 2 == 0);
        var middlePoint = maxPosition / 2 + 1;
        return middlePoint;
    }
}