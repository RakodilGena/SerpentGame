using Serpent.Server.GameProcessors.Models.Snakes.Segments.Base;
using Serpent.Server.GameProcessors.Models.Snakes.Segments.Directions;

namespace Serpent.Server.GameProcessors.Models.Snakes.Segments;

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
    /// To create segment that follows head.  
    /// </summary>
    /// <param name="headSegment"></param>
    /// <param name="direction"></param>
    public SnakeBodySegmentDomain(
        SnakeHeadSegmentDomain headSegment,
        SnakeBodySegmentDirectionTypeDomain direction) 
        : base(headSegment.X, headSegment.Y)
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