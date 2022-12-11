namespace Day8TreetopTreeHouse;

internal readonly record struct DirectionVector(int Dx, int Dy)
{
    public static readonly DirectionVector Down = new(0, 1);
    public static readonly DirectionVector Left = new(-1, 0);
    public static readonly DirectionVector Up = new(0, -1);
    public static readonly DirectionVector Right = new(1, 0);
}
