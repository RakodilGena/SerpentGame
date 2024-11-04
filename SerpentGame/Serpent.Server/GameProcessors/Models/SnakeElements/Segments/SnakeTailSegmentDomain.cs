﻿using Serpent.Server.GameProcessors.Models.SnakeElements.Segments.Base;

namespace Serpent.Server.GameProcessors.Models.SnakeElements.Segments;

internal sealed class SnakeTailSegmentDomain : SnakeSegmentOneDirectionDomain
{
    public SnakeTailSegmentDomain(
        int x, 
        int y, 
        SnakeDirectionTypeDomain direction) 
        : base(x, y, direction)
    {
    }

    public void SetLocationTo(
        SnakeBodySegmentDomain bodySegment,
        SnakeDirectionTypeDomain direction)
    {
        CopyPosition(bodySegment);
        Direction = direction;
    }
}