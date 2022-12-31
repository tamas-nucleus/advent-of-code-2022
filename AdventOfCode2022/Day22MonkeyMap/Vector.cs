namespace Day22MonkeyMap;

internal record struct Vector(
    int Dx,
    int Dy)
{
    public static readonly Vector Right = new(1, 0);
    public static readonly Vector Down = new(0, 1);
    public static readonly Vector Left = new(-1, 0);
    public static readonly Vector Up = new(0, -1);

    public Vector Turn(HandDirection direction)
    {
        return direction == HandDirection.Left
            ? new(Dy, -Dx)
            : new(-Dy, Dx);
    }

    public static Coordinates operator +(
        Coordinates coordinates,
        Vector vector)
    {
        return new(
            coordinates.X + vector.Dx, 
            coordinates.Y + vector.Dy);
    }

    public Vector3D ToVector3D()
    {
        return new(Dx, Dy, 0);
    }

    public override string ToString()
    {
        return $"{Dx},{Dy}";
    }
}
