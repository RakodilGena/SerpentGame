using Serpent.Server.GameProcessors.Models.SnakeElements.Segments.Base;
using Serpent.Server.GameProcessors.Models.SnakeElements.Segments.Directions;

namespace Serpent.Server.GameProcessors.Models.SnakeElements.Segments;

internal sealed class SnakeBodySegmentDomain : SnakeSegmentDomain
{
    public SnakeBodySegmentDirectionTypeDomain Direction { get; private set; }

    public SnakeBodySegmentDomain(
        int x, 
        int y, 
        SnakeBodySegmentDirectionTypeDomain direction) 
        : base(x, y)
    {
        Direction = direction;
    }

    /// <summary>
    /// To follow head.
    /// </summary>
    /// <param name="headSegment"></param>
    /// <param name="direction"></param>
    public void SetLocationTo(
        SnakeHeadSegmentDomain headSegment, 
        SnakeBodySegmentDirectionTypeDomain direction)
    {
        Direction = direction;
        CopyPosition(headSegment);
    }

    /// <summary>
    /// To follow next body segment.
    /// </summary>
    /// <param name="bodySegment"></param>
    public void CopyLocation(SnakeBodySegmentDomain bodySegment)
    {
        Direction = bodySegment.Direction;
        CopyPosition(bodySegment);
    }
}