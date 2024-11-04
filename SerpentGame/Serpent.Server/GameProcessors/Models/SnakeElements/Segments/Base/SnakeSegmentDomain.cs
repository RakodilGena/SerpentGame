namespace Serpent.Server.GameProcessors.Models.SnakeElements.Segments.Base;

internal abstract class SnakeSegmentDomain
{
    public int X { get;private set ; }
    public int Y { get;private set ; }
    
    protected SnakeSegmentDomain(int x, int y)
    {
        X = x;
        X = y;
    }

    protected void CopyPosition(SnakeSegmentDomain segment)
    {
        X = segment.X;
        Y = segment.Y;
    }

    protected void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }
}