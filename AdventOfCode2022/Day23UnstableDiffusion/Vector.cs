namespace Day23UnstableDiffusion;

internal record struct Vector(
    int X,
    int Y)
{
    public static readonly Vector North = new(0, -1);
    public static readonly Vector NorthEast = new(1, -1);
    public static readonly Vector East = new(1, 0);
    public static readonly Vector SouthEast = new(1, 1);
    public static readonly Vector South = new(0, 1);
    public static readonly Vector SouthWest = new(-1, 1);
    public static readonly Vector West = new(-1, 0);
    public static readonly Vector NorthWest = new(-1, -1);

    public static readonly IReadOnlyList<Vector> AllDirections = new Vector[] 
    { 
        North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest 
    };

    public static Vector operator +(Vector left, Vector right)
    {
        return new(left.X + right.X, left.Y + right.Y);
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}
