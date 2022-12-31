using System.Text;

namespace Day22MonkeyMap;

internal sealed class Matrix
{
    public Matrix()
    {
        Array = new decimal[3 * 3];
    }

    public Matrix(params decimal[] components)
    {
        Array = components;
    }

    private decimal[] Array { get; }

    public decimal this[int x, int y]
    {
        get { return Array[y * 3 + x]; }
        set { Array[y * 3 + x] = value; }
    }

    public Matrix Transpose()
    {
        return new Matrix(
            this[0, 0], this[0, 1], this[0, 2],
            this[1, 0], this[1, 1], this[1, 2],
            this[2, 0], this[2, 1], this[2, 2]);
    }

    public static Matrix operator *(Matrix left, Matrix right)
    {
        var result = new Matrix();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                decimal component = 0;
                for (int i = 0; i < 3; i++)
                {
                    component += left[i, y] * right[x, i];
                }
                result[x, y] = component;
            }
        }
        return result;
    }

    public static Vector3D operator *(Matrix matrix, Vector3D vector)
    {
        return new(
            X: matrix[0, 0] * vector.X + matrix[1, 0] * vector.Y + matrix[2, 0] * vector.Z,
            Y: matrix[0, 1] * vector.X + matrix[1, 1] * vector.Y + matrix[2, 1] * vector.Z,
            Z: matrix[0, 2] * vector.X + matrix[1, 2] * vector.Y + matrix[2, 2] * vector.Z);
    }

    public static Vector3D operator *(Vector3D vector, Matrix matrix)
    {
        return new(
            X: matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z,
            Y: matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z,
            Z: matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        for (int i = 0; i < 9; i += 3)
        {
            builder.AppendLine(string.Join(", ", Array[i..(i + 3)]));
        }
        return builder.ToString();
    }
}
