namespace Day9RopeBridge;

internal readonly record struct Vector(int Dx, int Dy)
{
    public static readonly Vector Down = new(0, -1);
    public static readonly Vector Left = new(-1, 0);
    public static readonly Vector Up = new(0, 1);
    public static readonly Vector Right = new(1, 0);

    public int Length => Math.Max(Math.Abs(Dx), Math.Abs(Dy));
}
