using System.Collections;

namespace Day8TreetopTreeHouse;

internal sealed class TreeMap
{
    private const int MaxTreeHeight = 9;
    // For each tree, the height of the tree.
    private readonly byte[] treeArray;
    // For each tree, whether it is visible from outside.
    private readonly BitArray outsideVisibilityArray;
    // For each tree, the number of trees visible from it in each of the
    // four directions: up, right, down, left. Each number is stored on
    // two bytes, from right to left in the order the directions are
    // listed above.
    private readonly ulong[] visibleTreeCountsArray;

    public TreeMap(
        int width,
        int height)
    {
        Width = width;
        Height = height;
        int cellCount = width * height;
        treeArray = new byte[cellCount];
        outsideVisibilityArray = new BitArray(cellCount);
        visibleTreeCountsArray = new ulong[cellCount];
    }

    public int Width { get; }
    public int Height { get; }

    public byte this[Coordinates coordinates]
    {
        get { return treeArray[ToArrayIndex(coordinates)]; }
        set { treeArray[ToArrayIndex(coordinates)] = value; }
    }

    public void SetRow(int y, byte[] row)
    {
        int startIndex = ToArrayIndex(new Coordinates(0, y));
        Array.Copy(row, 0, treeArray, startIndex, Width);
    }

    public (Coordinates Coordinates, int Score) FindMaxScenicScore()
    {
        var helperArray = new short[MaxTreeHeight + 1];
        for (int x = 0; x < Width; x++)
        {
            CaclulatScenicScores(new Coordinates(x, 0), Height, DirectionVector.Down, 0 * 16, helperArray);
            CaclulatScenicScores(new Coordinates(x, Height - 1), Height, DirectionVector.Up, 2 * 16, helperArray);
        }
        for (int y = 0; y < Height; y++)
        {
            CaclulatScenicScores(new Coordinates(0, y), Width, DirectionVector.Right, 3 * 16, helperArray);
            CaclulatScenicScores(new Coordinates(Width - 1, y), Width, DirectionVector.Left, 1 * 16, helperArray);
        }

        const ulong MaskUp = ushort.MaxValue;
        const ulong MaskRight = ((ulong)ushort.MaxValue) << 1 * 16;
        const ulong MaskDown = ((ulong)ushort.MaxValue) << 2 * 16;
        const ulong MaskLeft = ((ulong)ushort.MaxValue) << 3 * 16;

        int bestScore = -1;
        Coordinates bestCoordinates = default;
        for (int x = 1; x < Width - 1; x++)
        {
            for (int y = 1; y < Height - 1; y++)
            {
                var coordinates = new Coordinates(x, y);
                ulong visibleTreeCounts = visibleTreeCountsArray[ToArrayIndex(coordinates)];
                int score = (int)(
                    (visibleTreeCounts & MaskUp)
                    * ((visibleTreeCounts & MaskRight) >> (1 * 16))
                    * ((visibleTreeCounts & MaskDown) >> (2 * 16))
                    * ((visibleTreeCounts & MaskLeft) >> (3 * 16)));
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCoordinates = coordinates;
                }
            }
        }
        return (bestCoordinates, bestScore);
    }

    public int CalculateOutsideVisibility()
    {
        for (int x = 0; x < Width; x++)
        {
            var top = new Triplet
            {
                Direction = DirectionVector.Down,
                Coordinates = new Coordinates(x, -1),
                MaxTreeHeight = int.MinValue
            };
            var bottom = new Triplet
            {
                Direction = DirectionVector.Up,
                Coordinates = new Coordinates(x, Height),
                MaxTreeHeight = int.MinValue
            };
            CloseJaws(top, bottom);
        }
        for (int y = 0; y < Height; y++)
        {
            var left = new Triplet
            {
                Direction = DirectionVector.Right,
                Coordinates = new Coordinates(-1, y),
                MaxTreeHeight = int.MinValue
            };
            var right = new Triplet
            {
                Direction = DirectionVector.Left,
                Coordinates = new Coordinates(Width, y),
                MaxTreeHeight = int.MinValue
            };
            CloseJaws(left, right);
        }

        int visibleCount = 0;
        for (int i = 0; i < outsideVisibilityArray.Length; i++)
        {
            if (outsideVisibilityArray[i])
            {
                visibleCount++;
            }
        }
        return visibleCount;
    }

    private void CaclulatScenicScores(
        Coordinates startEdgeCell,
        int stepCount,
        DirectionVector direction,
        int shiftCount,
        short[] helperArray)
    {
        Array.Clear(helperArray);
        var coordinates = startEdgeCell;
        for (int step = 0; step < stepCount; step++)
        {
            int arrayIndex = ToArrayIndex(coordinates);
            int treeHeight = treeArray[arrayIndex];
            short visibleTreeCount = helperArray[treeHeight];
            visibleTreeCountsArray[arrayIndex] |= (ulong)visibleTreeCount << shiftCount;
            Array.Fill(helperArray, (short)1, 0, treeHeight + 1);
            for (int i = treeHeight + 1; i < MaxTreeHeight; i++)
            {
                helperArray[i]++;
            }
            coordinates += direction;
        }
    }

    private void CloseJaws(Triplet t1, Triplet t2)
    {
        var toMove = t1.MaxTreeHeight < t2.MaxTreeHeight ? t1 : t2;
        while (true)
        {
            toMove.Coordinates += toMove.Direction;
            if (t1.Coordinates == t2.Coordinates)
            {
                break;
            }

            int index = ToArrayIndex(toMove.Coordinates);
            int treeHeight = treeArray[index];
            if (treeHeight <= toMove.MaxTreeHeight)
            {
                continue;
            }

            outsideVisibilityArray[index] = true;
            toMove.MaxTreeHeight = treeHeight;
            toMove = t1.MaxTreeHeight < t2.MaxTreeHeight ? t1 : t2;
        }
    }

    private int ToArrayIndex(Coordinates coordinates)
    {
        return coordinates.Y * Width + coordinates.X;
    }

    private sealed class Triplet
    {
        public DirectionVector Direction { get; init; }
        public Coordinates Coordinates { get; set; }
        public int MaxTreeHeight { get; set; }
    }
}
