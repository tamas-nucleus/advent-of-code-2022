namespace Day17PyroclasticFlow;

internal record struct Vector(
    int Dx,
    int Dy)
{
    public static Coordinates operator +(
        Coordinates coordinates, 
        Vector vector)
    {
        return new Coordinates(
            coordinates.X + vector.Dx, 
            coordinates.Y + vector.Dy);
    }
}
