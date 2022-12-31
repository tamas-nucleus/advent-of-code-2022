using System.Text;

namespace Day22MonkeyMap;

internal sealed class Face
{
    public static readonly Vector3D BaseOffset = new(0, 0, -1m);

    public Face(
        Coordinates inputOffset,
        int size,
        Matrix rotation)
    {
        InputOffset = inputOffset;
        Size = size;
        Cells = new bool[size * size];
        Rotation = rotation;
        RotationBack = Rotation.Transpose();
        decimal halfSize = Size / 2m;
        Offset3D = (BaseOffset * Rotation) * halfSize;
        if (Offset3D.X < 0) { Offset3D = new Vector3D(Offset3D.X - 1, 0, 0); }
        else if (Offset3D.Y < 0) { Offset3D = new Vector3D(0, Offset3D.Y - 1, 0); }
        else if (Offset3D.Z < 0) { Offset3D = new Vector3D(0, 0, Offset3D.Z - 1); }

        var checkX = new Vector3D(1, 0, 0) * Rotation;
        decimal adjustmentX = HasNegativeComponent(checkX) ? -1 : 0;
        var checkY = new Vector3D(0, 1, 0) * Rotation;
        decimal adjustmentY = HasNegativeComponent(checkY) ? -1 : 0;
        Offset2D = new Vector3D(halfSize + adjustmentX, halfSize + adjustmentY, 0);
    }

    public Coordinates InputOffset { get; }
    public int Size { get; }
    public bool[] Cells { get; }
    public Matrix Rotation { get; }
    public Matrix RotationBack { get; }
    public Vector3D Offset3D { get; }
    public Vector3D Offset2D { get; }
    public string Name
    {
        get
        {
            if (Offset3D.X < 0) { return "Left"; }
            else if (Offset3D.X > 0) { return "Right"; }
            else if (Offset3D.Y < 0) { return "Back"; }
            else if (Offset3D.Y > 0) { return "Front"; }
            else if (Offset3D.Z < 0) { return "Top"; }
            else { return "Bottom"; }
        }
    }

    public bool this[Coordinates coordinates]
    {
        get => Cells[GetCellIndex(coordinates.X, coordinates.Y)];   
    }

    public Vector3D ToLocal(Vector3D point)
    {
        return (point - Offset3D) * RotationBack + Offset2D;
    }

    public Vector3D ToGlobal(Coordinates point)
    {
        return ToGlobal(new Vector3D(point.X, point.Y, 0));
    }

    public Vector3D ToGlobal(Vector vector)
    {
        return ToGlobal(new Vector3D(vector.Dx, vector.Dy, 0));
    }

    public Vector3D ToGlobal(Vector3D point)
    {
        return (point - Offset2D) * Rotation + Offset3D;
    }

    public bool IsBorderPoint(Vector3D point)
    {
        var local = ToLocal(point);
        return local.Z == 0
            && (((local.X == -1 || local.X == Size) && local.Y >= 0 && local.Y < Size)
                || ((local.Y == -1 || local.Y == Size) && local.X >= 0 && local.X < Size));

    }

    public IEnumerable<Vector3D> GetBorderPoints()
    {
        for (int x = 0; x < Size; x++)
        {
            yield return ToGlobal(new Vector3D(x, -1, 0));
            yield return ToGlobal(new Vector3D(x, Size, 0));
        }
        for (int y = 0; y < Size; y++)
        {
            yield return ToGlobal(new Vector3D(-1, y, 0));
            yield return ToGlobal(new Vector3D(Size, y, 0));
        }
    }

    public IEnumerable<Vector3D> GetPoints()
    {
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                yield return ToGlobal(new Vector3D(x, y, 0));
            }
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"{Name} face:");
        int index = 0;
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                builder.Append(Cells[index++] ? '.' : '#');
            }
            builder.AppendLine();
        }
        return builder.ToString();
    }

    private int GetCellIndex(int x, int y)
    {
        return y * Size + x;
    }

    private static bool HasNegativeComponent(Vector3D vector)
    {
        return vector.X < 0 || vector.Y < 0 || vector.Z < 0;
    }
}
