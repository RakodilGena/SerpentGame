namespace Serpent.Server.GameProcessors.Models.Snakes.Segments.Base;

internal abstract class SnakeSegmentOneDirectionDomain : SnakeSegmentDomain
{
    public SnakeDirectionTypeDomain Direction { get; protected set; }

    protected SnakeSegmentOneDirectionDomain(
        int x, 
        int y, 
        SnakeDirectionTypeDomain direction) 
        : base(x, y)
    {
        Direction = direction;
    }
}