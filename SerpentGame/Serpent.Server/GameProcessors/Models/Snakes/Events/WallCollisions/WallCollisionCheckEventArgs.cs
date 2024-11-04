namespace Serpent.Server.GameProcessors.Models.Snakes.Events.WallCollisions;

internal sealed class WallCollisionCheckEventArgs
{
    public int X { get; }
    public int Y { get; }
    public WallCollisionResultDomain? WallCollisionResult { get; private set; }

    public WallCollisionCheckEventArgs(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetResult(WallCollisionResultDomain wallCollisionResult)
    {
        WallCollisionResult = wallCollisionResult;
    }
}