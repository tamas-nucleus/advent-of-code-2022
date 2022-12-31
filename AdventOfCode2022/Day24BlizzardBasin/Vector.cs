namespace Day24BlizzardBasin;

internal record struct Vector(
    int X,
    int Y)
{
    public static readonly Vector Zero = new();
    public static readonly Vector Up = new(0, -1);
    public static readonly Vector Right = new(1, 0);
    public static readonly Vector Down = new(0, 1);
    public static readonly Vector Left = new(-1, 0);

    public static Vector operator +(Vector left, Vector right)
    {
        return new(left.X + right.X, left.Y + right.Y);
    }
}
