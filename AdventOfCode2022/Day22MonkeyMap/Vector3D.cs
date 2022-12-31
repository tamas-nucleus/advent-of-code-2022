namespace Day22MonkeyMap;

internal record struct Vector3D(
    decimal X,
    decimal Y,
    decimal Z)
{
    public static readonly Vector3D Origin = new();

    public decimal this[int x]
    {
        get 
        {
            return x switch
            {
                0 => X,
                1 => Y,
                _ => Z
            };
        }
    }

    public Vector Project2DVector()
    {
        return new((int)X, (int)Y);
    }

    public Coordinates Project2DCoordinates()
    {
        return new((int)X, (int)Y);
    }

    public static Vector3D operator +(Vector3D left, Vector3D right)
    {
        return new(
            X: left.X + right.X, 
            Y: left.Y + right.Y,
            Z: left.Z + right.Z);
    }

    public static Vector3D operator -(Vector3D left, Vector3D right)
    {
        return new(
            X: left.X - right.X,
            Y: left.Y - right.Y,
            Z: left.Z - right.Z);
    }

    public static Vector3D operator *(Vector3D vector, decimal scalar)
    {
        return new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
    }
}
