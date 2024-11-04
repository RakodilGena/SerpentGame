using System.Diagnostics;
using System.Runtime.InteropServices;
using Serpent.Server.GameProcessors.GameSessions;
using Serpent.Server.GameProcessors.Models.GameSettings;
using Serpent.Server.GameProcessors.Models.Snakes.Commands;
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

    public event EventHandler? CollisionHappened;

    public SnakeDomain(
        SnakeHeadSegmentDomain head,
        List<SnakeBodySegmentDomain> body,
        SnakeTailSegmentDomain tail)
    {
        _head = head;
        _body = body;
        _tail = tail;
        _direction = head.Direction;
    }

    public void Move(SnakeMoveNoEatCommand command)
    {
        MoveTail();
        MoveBody(nextHeadDirection: command.Direction);
        
        var success = TryMoveHead(
            command.FieldSettings, 
            command.Direction);

        //collided with walls
        if (!success)
        {
            InvokeCollision();
            return;
        }

        success = EnsureHeadNotCollidedBody();
        if (success) 
            return;
        
        //collided with body
        InvokeCollision();
    }

    private bool EnsureHeadNotCollidedBody()
    {
        Span<(int x, int y)> collisionSpan =
        [
            .._body.Select(segment => (segment.X, segment.Y)),
            (_tail.X, _tail.Y)
        ];
        return collisionSpan.Contains((_head.X, _head.Y)) is false;
    }

    public void Move(SnakeMoveEatSnackCommand command)
    {
        //dont move body and tail, enlarge snake with 1 segment.
        var direction = GetDirectionForSegmentFollowingHead(command.Direction);
        var newSegment = new SnakeBodySegmentDomain(_head, direction);
        
        var success = TryMoveHead(
            command.FieldSettings, 
            command.Direction);
        
        //collided with walls
        if (!success)
        {
            InvokeCollision();
            return;
        }
        
        //no collision, enlarge body with new segment
        _body.Insert(0, newSegment);
    }

    private void Move(SnakeMoveEatScissorsCommand command)
    {
        //cut as many requested segments as possible and do plain move without checking for body collisions.
        CutBody(command.RequestedSegmentsToCut);
        
        MoveTail();
        MoveBody(nextHeadDirection: command.Direction);
        
        var success = TryMoveHead(
            command.FieldSettings, 
            command.Direction);

        if (success) 
            return;
        
        //collided with walls
        InvokeCollision();
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
    /// <param name="nextHeadDirection"></param>
    /// <exception cref="ArgumentException"></exception>
    private void MoveBody(SnakeDirectionTypeDomain nextHeadDirection)
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
        var bodyTopDirection = GetDirectionForSegmentFollowingHead(nextHeadDirection);
        bodyTop.SetLocationTo(_head, bodyTopDirection);
    }

    private SnakeBodySegmentDirectionTypeDomain GetDirectionForSegmentFollowingHead(
        SnakeDirectionTypeDomain nextHeadDirection)
    {
        var prevHeadDirection = _head.Direction;

        return (prevHeadDirection, nextHeadDirection) switch
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

    /// <summary>
    /// Should be launched after body and tail movement.
    /// Checks collision with walls and with the rest of the snake (optionally)
    /// </summary>
    /// <param name="fieldSettings"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private bool TryMoveHead(
        FieldSettings fieldSettings,
        SnakeDirectionTypeDomain direction)
    {
        var success = TryGetNewHeadPosition(fieldSettings, direction, out var x, out var y);
        if (!success)
            return false;
        
        _head.Move(x,y, direction);
        return true;
    }

    private bool TryGetNewHeadPosition(
        FieldSettings fieldSettings,
        SnakeDirectionTypeDomain direction,
        out int x, 
        out int y)
    {
        (x, y) = (_head.X, _head.Y);

        switch (direction)
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

        //check for walls being hit.
        if (x < 0 || x > fieldSettings.MaxPosition)
        {
            if (fieldSettings.WallsTransparent is false)
            {
                return false;
            }
            FixPosition(ref x, fieldSettings.MaxPosition);
        }
        else
        {
            //else because X and Y can't exceed boundaries simultaneously
            if (y < 0 || y > fieldSettings.MaxPosition)
            {
                if (fieldSettings.WallsTransparent is false)
                {
                    return false;
                }
                FixPosition(ref y, fieldSettings.MaxPosition);
            }
        }

        return true;
    }

    private static void FixPosition(ref int dimension, int maxValue)
    {
        if (dimension < 0)
        {
            dimension = 0;
            return;
        }

        Debug.Assert(dimension > maxValue);
        dimension = maxValue;
    }

    private void SetNewDirection(UserInputDomain input)
    {
        if (input is UserInputDomain.None)
            return;

        if (input
                is UserInputDomain.Up
                or UserInputDomain.Down
            && _direction
                is SnakeDirectionTypeDomain.Up
                or SnakeDirectionTypeDomain.Down
            ||
            input
                is UserInputDomain.Left
                or UserInputDomain.Right
            && _direction
                is SnakeDirectionTypeDomain.Left
                or SnakeDirectionTypeDomain.Right)
            return;

        _direction = input switch
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

    private void InvokeCollision()
    {
        CollisionHappened?.Invoke(this, EventArgs.Empty);
    }

    public IReadOnlyList<SnakeSegmentDomain> AllSegments => [_head, .._body, _tail];
}