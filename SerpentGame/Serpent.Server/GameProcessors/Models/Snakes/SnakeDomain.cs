using System.Diagnostics;
using System.Runtime.InteropServices;
using Serpent.Server.GameProcessors.GameSessions;
using Serpent.Server.GameProcessors.Models.Consumables.Concrete;
using Serpent.Server.GameProcessors.Models.Snakes.Events.ConsumeChecks;
using Serpent.Server.GameProcessors.Models.Snakes.Events.WallCollisions;
using Serpent.Server.GameProcessors.Models.Snakes.Segments;
using Serpent.Server.GameProcessors.Models.Snakes.Segments.Base;
using Serpent.Server.GameProcessors.Models.Snakes.Segments.Directions;

namespace Serpent.Server.GameProcessors.Models.Snakes;

internal sealed class SnakeDomain
{
    private readonly SnakeHeadSegmentDomain _head;
    private readonly SnakeTailSegmentDomain _tail;
    private readonly List<SnakeBodySegmentDomain> _body;
    private SnakeDirectionTypeDomain _direction;
    
    private const int MinSegmentsInBody = 3;
    public event WallCollisionCheckHandler? WallCollisionCheck;

    public event EventHandler? BodyCollided;
    public event ConsumeCheckHandler? ConsumeCheck;

    public SnakeDomain(
        SnakeHeadSegmentDomain head,
        List<SnakeBodySegmentDomain> body,
        SnakeTailSegmentDomain tail)
    {
        _head = head;
        _body = body;
        _tail = tail;
        _direction = _head.Direction;
    }

    public void Move(UserInputDomain userInput)
    {
        _direction = GetNewDirection(userInput);
        
        //got direction. now get new head position
        var success = TryGetNewHeadPosition(
            out var x, 
            out var y);

        if (!success)
            return;
        
        var newHeadPosition = new NewHeadPosition(x, y);
        
        //if not dead, check whether snake ate something
        var consumeResult = CheckIfSomethingConsumed(newHeadPosition);

        //depending on the result decide method to move.
        switch (consumeResult.SegmentsDiff)
        {
            case 0:
                MoveNoEaten(newHeadPosition);
                return;
            
            case > 0:
                MoveEatenApple(newHeadPosition);
                return;
            
            case < 0:
                var segmentsToCut = -consumeResult.SegmentsDiff;
                MoveEatenScissors(newHeadPosition, segmentsToCut);
                break; 
        }
    }

    private void MoveNoEaten(NewHeadPosition newHeadPosition)
    {
        MoveTail();
        MoveBody();
        MoveHead(newHeadPosition);
        
        //todo log move
        CheckIfHeadNotCollidedBody();
    }

    private void CheckIfHeadNotCollidedBody()
    {
        Span<(int x, int y)> collisionSpan =
        [
            .._body.Select(segment => (segment.X, segment.Y)),
            (_tail.X, _tail.Y)
        ];
        
        var collided =collisionSpan.Contains((_head.X, _head.Y));
        
        if (collided)
            BodyCollided?.Invoke(this, EventArgs.Empty);
        
        //todo log collide
    }

    private void MoveEatenApple(NewHeadPosition newHeadPosition)
    {
        //dont move body and tail, enlarge snake with 1 segment.
        var direction = GetDirectionForSegmentFollowingHead();
        var newSegment = new SnakeBodySegmentDomain(_head, direction);
        
        //no body collision possible, enlarge body with new segment
        _body.Insert(0, newSegment);
        
        MoveHead(newHeadPosition);
        //todo log move
    }

    private void MoveEatenScissors(NewHeadPosition newHeadPosition, int segmentsToCut)
    {
        Debug.Assert(segmentsToCut > 0);
        //cut as many requested segments as possible and do plain move without checking for body collisions.
        CutBody(segmentsToCut);
        
        MoveTail();
        MoveBody();
        MoveHead(newHeadPosition);
        //todo log move
        
        //no body collision possible
    }

    private void CutBody(int requestedSegmentsToCut)
    {
        Debug.Assert(requestedSegmentsToCut >= 0);
        
        var maxSegmentsToCut = Math.Min(requestedSegmentsToCut, _body.Count - MinSegmentsInBody);
        if (maxSegmentsToCut <= 0)
            return;

        var indexToCutFrom = _body.Count - maxSegmentsToCut;
        
        _body.RemoveRange(indexToCutFrom, maxSegmentsToCut);
    }

    
    /// <summary>
    /// Should be launched before head and body movement.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void MoveTail()
    {
        var lastBodySegment = _body[^1];

        var tail = _tail;

        var direction = lastBodySegment.Direction switch
        {
            SnakeBodySegmentDirectionTypeDomain.Up
                or SnakeBodySegmentDirectionTypeDomain.LeftUp
                or SnakeBodySegmentDirectionTypeDomain.RightUp
                => SnakeDirectionTypeDomain.Up,

            SnakeBodySegmentDirectionTypeDomain.Down
                or SnakeBodySegmentDirectionTypeDomain.LeftDown
                or SnakeBodySegmentDirectionTypeDomain.RightDown
                => SnakeDirectionTypeDomain.Down,

            SnakeBodySegmentDirectionTypeDomain.Left
                or SnakeBodySegmentDirectionTypeDomain.UpLeft
                or SnakeBodySegmentDirectionTypeDomain.DownLeft
                => SnakeDirectionTypeDomain.Left,

            SnakeBodySegmentDirectionTypeDomain.Right
                or SnakeBodySegmentDirectionTypeDomain.UpRight
                or SnakeBodySegmentDirectionTypeDomain.DownRight
                => SnakeDirectionTypeDomain.Right,

            _ => throw new ArgumentOutOfRangeException(
                nameof(lastBodySegment.Direction),
                lastBodySegment.Direction,
                "invalid tail direction type.")
        };

        tail.SetLocationTo(lastBodySegment, direction);
    }

    /// <summary>
    /// Should be launched before head and after tail movement.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    private void MoveBody()
    {
        ReadOnlySpan<SnakeBodySegmentDomain> span = CollectionsMarshal.AsSpan(_body);

        //skips first segment of body which follows head.
        for (int i = span.Length - 1; i > 0; i--)
        {
            var current = _body[i];
            var next = _body[i - 1];

            current.CopyLocation(next);
        }

        var bodyTop = span[0];
        var bodyTopDirection = GetDirectionForSegmentFollowingHead();
        bodyTop.SetLocationTo(_head, bodyTopDirection);
    }

    private SnakeBodySegmentDirectionTypeDomain GetDirectionForSegmentFollowingHead()
    {
        var prevHeadDirection = _head.Direction;

        return (prevHeadDirection, _direction) switch
        {
            (SnakeDirectionTypeDomain.Up, SnakeDirectionTypeDomain.Up)
                => SnakeBodySegmentDirectionTypeDomain.Up,
            (SnakeDirectionTypeDomain.Left, SnakeDirectionTypeDomain.Up)
                => SnakeBodySegmentDirectionTypeDomain.LeftUp,
            (SnakeDirectionTypeDomain.Right, SnakeDirectionTypeDomain.Up)
                => SnakeBodySegmentDirectionTypeDomain.RightUp,

            (SnakeDirectionTypeDomain.Right, SnakeDirectionTypeDomain.Right)
                => SnakeBodySegmentDirectionTypeDomain.Right,
            (SnakeDirectionTypeDomain.Up, SnakeDirectionTypeDomain.Right)
                => SnakeBodySegmentDirectionTypeDomain.UpRight,
            (SnakeDirectionTypeDomain.Down, SnakeDirectionTypeDomain.Right)
                => SnakeBodySegmentDirectionTypeDomain.DownRight,

            (SnakeDirectionTypeDomain.Down, SnakeDirectionTypeDomain.Down)
                => SnakeBodySegmentDirectionTypeDomain.Down,
            (SnakeDirectionTypeDomain.Left, SnakeDirectionTypeDomain.Down)
                => SnakeBodySegmentDirectionTypeDomain.LeftDown,
            (SnakeDirectionTypeDomain.Right, SnakeDirectionTypeDomain.Down)
                => SnakeBodySegmentDirectionTypeDomain.RightDown,

            (SnakeDirectionTypeDomain.Left, SnakeDirectionTypeDomain.Left)
                => SnakeBodySegmentDirectionTypeDomain.Left,
            (SnakeDirectionTypeDomain.Up, SnakeDirectionTypeDomain.Left)
                => SnakeBodySegmentDirectionTypeDomain.UpLeft,
            (SnakeDirectionTypeDomain.Down, SnakeDirectionTypeDomain.Left)
                => SnakeBodySegmentDirectionTypeDomain.DownLeft,

            _ => throw new ArgumentException("head prev & next directions were out of range.")
        };
    }

    private SnakeDirectionTypeDomain GetNewDirection(UserInputDomain input)
    {
        var direction = _direction;
        
        if (input is UserInputDomain.None)
            return direction;

        if (input
                is UserInputDomain.Up
                or UserInputDomain.Down
            && direction
                is SnakeDirectionTypeDomain.Up
                or SnakeDirectionTypeDomain.Down
            ||
            input
                is UserInputDomain.Left
                or UserInputDomain.Right
            && direction
                is SnakeDirectionTypeDomain.Left
                or SnakeDirectionTypeDomain.Right)
            return direction;

        return input switch
        {
            UserInputDomain.Up => SnakeDirectionTypeDomain.Up,
            UserInputDomain.Down => SnakeDirectionTypeDomain.Down,
            UserInputDomain.Left => SnakeDirectionTypeDomain.Left,
            UserInputDomain.Right => SnakeDirectionTypeDomain.Right,
            
            _ => throw new ArgumentOutOfRangeException(
                nameof(input), 
                input, 
                "user input is out of range and pot processed.")
        };
    }

    /// <summary>
    /// Returns result whether snake head will collide with walls,
    /// and new position if everything's OK.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool TryGetNewHeadPosition(
        out int x, 
        out int y)
    {
        (x, y) = (_head.X, _head.Y);

        switch (_direction)
        {
            case SnakeDirectionTypeDomain.Up:
                y--;
                break;
            case SnakeDirectionTypeDomain.Left:
                x--;
                break;
            case SnakeDirectionTypeDomain.Right:
                x++;
                break;
            case SnakeDirectionTypeDomain.Down:
                y++;
                break;
        }

        var args = new WallCollisionCheckEventArgs(x, y);
        WallCollisionCheck?.Invoke(args);
        
        Debug.Assert(args.WallCollisionResult is not null);
        var collisionResult = args.WallCollisionResult.Value;

        switch (collisionResult.Type)
        {
            case WallCollisionResultTypeDomain.Break:
                return false;
            
            case WallCollisionResultTypeDomain.Teleport:
                (x, y) = (collisionResult.NewX, collisionResult.NewY);
                break;
        }

        return true;
    }

    private ConsumeResult CheckIfSomethingConsumed(NewHeadPosition newHeadPosition)
    {
        var args = new ConsumeCheckEventArgs(
            newHeadPosition.X, 
            newHeadPosition.Y);
        
        ConsumeCheck?.Invoke(args);

        if (!args.Processed)
            return ConsumeResult.None();

        //then check what's eaten
        return args.CollidedConsumable switch
        {
            CommonAppleDomain or GoldenAppleDomain => ConsumeResult.Eaten(),
            ScissorsDomain s => ConsumeResult.Cut(s.SegmentsToCut),

            _ => throw new ArgumentOutOfRangeException(
                nameof(args.CollidedConsumable),
                args.CollidedConsumable,
                "consumable type is out of range and can't be processed.")
        };
    }

    /// <summary>
    /// Should be launched after body and tail movement.
    /// Checks collision with walls and with the rest of the snake (optionally)
    /// </summary>
    /// <returns></returns>
    private void MoveHead(NewHeadPosition position)
    {
        _head.Move(position.X, position.Y, _direction);
    }

    public IReadOnlyList<SnakeSegmentDomain> AllSegments => [_head, .._body, _tail];
    
    private readonly record struct NewHeadPosition(int X, int Y);
    
    private readonly record struct ConsumeResult(
        int SegmentsDiff)
    {
        public static ConsumeResult None() => new(0);

        public static ConsumeResult Eaten() => new(1);

        public static ConsumeResult Cut(int segmentsToCut) => new(-segmentsToCut);
    }
}