using Serpent.Server.GameProcessors.Models.SnakeElements.Segments.Base;

namespace Serpent.Server.GameProcessors.Models.SnakeElements.Segments;

internal sealed class SnakeHeadSegmentDomain : SnakeSegmentOneDirectionDomain
{
    public SnakeHeadSegmentDomain(
        int x, 
        int y, 
        SnakeDirectionTypeDomain direction) 
        : base(x, y, direction)
    {
    }

    public void Move(int x, int y, SnakeDirectionTypeDomain direction)
    {
        Direction = direction;
        SetPosition(x, y);
    }
}