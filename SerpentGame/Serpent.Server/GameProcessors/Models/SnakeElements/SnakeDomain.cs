using System.Diagnostics;
using Serpent.Server.GameProcessors.GameSessions;
using Serpent.Server.GameProcessors.Models.SnakeElements.Segments;
using Serpent.Server.GameProcessors.Models.SnakeElements.Segments.Base;
using Serpent.Server.GameProcessors.Models.SnakeElements.Segments.Directions;

namespace Serpent.Server.GameProcessors.Models.SnakeElements;

internal sealed class SnakeDomain
{
    private SnakeHeadSegmentDomain _head;
    private SnakeTailSegmentDomain _tail;
    private List<SnakeBodySegmentDomain> _body;
    private SnakeDirectionTypeDomain _direction;

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

    public void MoveNoEat(SnakeDirectionTypeDomain direction)
    {
        
    }

    public void MoveEatSnack(SnakeDirectionTypeDomain direction)
    {
        
    }

    private void MoveEatScissors(SnakeDirectionTypeDomain direction)
    {
        
    }

    private void MoveTail()
    {
        var previousSegment = _body[^1];

        var tail = _tail;

        var direction = previousSegment.Direction switch
        {
            SnakeBodySegmentDirectionTypeDomain.LeftUp
                or SnakeBodySegmentDirectionTypeDomain.RightUp
                => SnakeDirectionTypeDomain.Up,

            SnakeBodySegmentDirectionTypeDomain.LeftDown
                or SnakeBodySegmentDirectionTypeDomain.RightDown
                => SnakeDirectionTypeDomain.Down,

            SnakeBodySegmentDirectionTypeDomain.UpLeft
                or SnakeBodySegmentDirectionTypeDomain.DownLeft
                => SnakeDirectionTypeDomain.Left,

            SnakeBodySegmentDirectionTypeDomain.UpRight
                or SnakeBodySegmentDirectionTypeDomain.DownRight
                => SnakeDirectionTypeDomain.Right,

            _ => throw new ArgumentOutOfRangeException(
                nameof(previousSegment.Direction),
                previousSegment.Direction,
                "invalid tail direction type.")
        };

        tail.SetLocationTo(previousSegment, direction);
    }

    private void MoveBody()
    {
        
    }

    private void MoveHead()
    {
        
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

    public IReadOnlyList<SnakeSegmentDomain> Body => _body;
}