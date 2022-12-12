namespace Day9RopeBridge;

internal readonly record struct Coordinates(int X, int Y)
{
    public static Coordinates operator +(Coordinates coordinates, Vector direction)
        => new(coordinates.X + direction.Dx, coordinates.Y + direction.Dy);

    public static Coordinates operator -(Coordinates coordinates, Vector direction)
    => new(coordinates.X - direction.Dx, coordinates.Y - direction.Dy);

    public static Vector operator -(Coordinates left, Coordinates right)
        => new(left.X - right.X, left.Y - right.Y);

    public int DistanceFrom(Coordinates other)
        => (other - this).Length;
}