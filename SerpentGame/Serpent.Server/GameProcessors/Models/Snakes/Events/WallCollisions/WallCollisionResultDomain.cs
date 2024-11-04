namespace Serpent.Server.GameProcessors.Models.Snakes.Events.WallCollisions;

internal readonly record struct WallCollisionResultDomain(
    WallCollisionResultTypeDomain Type,
    int NewX,
    int NewY)
{
    public static WallCollisionResultDomain NoCollision()
    {
        return new WallCollisionResultDomain(
            WallCollisionResultTypeDomain.None, 0, 0);
    }
    
    public static WallCollisionResultDomain Teleport(int newX, int newY)
    {
        return new WallCollisionResultDomain(
            WallCollisionResultTypeDomain.Teleport, newX, newY);
    }
    
    public static WallCollisionResultDomain Break()
    {
        return new WallCollisionResultDomain(
            WallCollisionResultTypeDomain.Break, 0, 0);
    }
};