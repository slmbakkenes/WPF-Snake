﻿namespace Snake;

public class Direction(int rowOffset, int colOffset)
{
    public static readonly Direction Left = new Direction(0, -1);
    public static readonly Direction Right = new Direction(0, 1);
    public static readonly Direction Up = new Direction(-1, 0);
    public static readonly Direction Down = new Direction(1, 0);
    
    public int RowOffset { get; set; } = rowOffset;
    public int ColOffset { get; set; } = colOffset;

    public Direction Opposite()
    {
        return new Direction(-RowOffset, -ColOffset);
    }

    public override bool Equals(object obj)
    {
        return obj is Direction direction &&
               RowOffset == direction.RowOffset &&
               ColOffset == direction.ColOffset;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(RowOffset, ColOffset);
    }

    public static bool operator ==(Direction left, Direction right)
    {
        return EqualityComparer<Direction>.Default.Equals(left, right);
    }

    public static bool operator !=(Direction left, Direction right)
    {
        return !(left == right);
    }
}