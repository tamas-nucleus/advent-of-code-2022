namespace Day8TreetopTreeHouse;

internal readonly record struct Coordinates(int X, int Y)
{
    public static Coordinates operator +(Coordinates coordinates, DirectionVector direction)
        => new(coordinates.X + direction.Dx, coordinates.Y + direction.Dy);
}
