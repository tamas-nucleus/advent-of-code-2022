using System.Diagnostics;

namespace Day22MonkeyMap;

internal sealed class Map
{
    public Map(
        int width, 
        int height,
        IEnumerable<Instruction> instructions)
    {
        this.Width = width;
        this.Height = height;
        Cells = new CellType[this.Height * this.Width];
        Instructions = instructions.ToList();
    }

    public int Width { get; }
    public int Height { get; }
    public CellType[] Cells { get; }
    public List<Instruction> Instructions { get; }
    
    public Coordinates Start { get; private set; }
    public Dictionary<Vector, Dictionary<Coordinates, Coordinates>> WarpGates { get; } = new();
    public Coordinates FinalPosition { get; private set; }
    public Vector FinalDirection { get; private set; }
    public int Password { get; private set; }

    public void FollowInstructions()
    {
        FindStart();
        PrepareWarpGates();

        var position = Start;
        var direction = Vector.Right;
        var warpGates = WarpGates[direction];
        foreach (var instruction in Instructions)
        {
            if (instruction is TurnInstruction turn)
            {
                direction = direction.Turn(turn.Direction);
                warpGates = WarpGates[direction];
            }
            else if (instruction is MoveInstruction move)
            {
                for (int step = 0; step < move.Distance; step++)
                {
                    var next = position + direction;
                    if (warpGates.TryGetValue(next, out var warpTarget))
                    {
                        next = warpTarget;
                    }

                    var nextCell = Cells[GetIndex(next)];
                    Debug.Assert(nextCell != CellType.Empty);
                    if (nextCell == CellType.Wall)
                    {
                        break;
                    }
                    position = next;
                }
            }
        }
        
        FinalPosition = position;
        FinalDirection = direction;

        int passwordRow = FinalPosition.Y + 1;
        int passwordColumn = FinalPosition.X + 1;
        int passwordDirection = GetPasswordDirection(FinalDirection);
        Password = 1000 * passwordRow + 4 * passwordColumn + passwordDirection;
    }

    public static Map Load(string path)
    {
        var lineArray = File.ReadAllLines(path);
        int height = lineArray.Length - 2;
        int width = lineArray.Take(height).Max(l => l.Length);

        var instructions = Instruction.Parse(lineArray[^1]);

        var map = new Map(width, height, instructions);
        int index = 0;
        var mapArray = map.Cells;
        for (int y = 0; y < height; y++)
        {
            string line = lineArray[y];
            for (int x = 0; x < line.Length; x++)
            {
                mapArray[index++] = line[x] switch
                {
                    '.' => CellType.Free,
                    '#' => CellType.Wall,
                    _ => CellType.Empty
                };
            }
            for (int x = line.Length; x < width; x++)
            {
                mapArray[index++] = CellType.Empty;
            }
        }

        return map;
    }

    private static int GetPasswordDirection(Vector vector)
    {
        if (vector == Vector.Right) { return 0; }
        else if (vector == Vector.Down) { return 1; }
        else if (vector == Vector.Left) { return 2; }
        else  { return 3; }
    }

    private void PrepareWarpGates()
    {
        WarpGates.Clear();
        WarpGates[Vector.Right] = new();
        WarpGates[Vector.Down] = new();
        WarpGates[Vector.Left] = new();
        WarpGates[Vector.Up] = new();

        for (int x = 0; x < Width; x++)
        {
            PrepareWarpGates(new Coordinates(x, 0), Height, Vector.Down);
        }
        for (int y = 0; y < Height; y++)
        {
            PrepareWarpGates(new Coordinates(0, y), Width, Vector.Right);
        }
    }

    private void PrepareWarpGates(
        Coordinates start,
        int size,
        Vector direction)
    {
        var oppositeDirection = new Vector(-1 * direction.Dx, -1 * direction.Dy);
        Coordinates position = start;
        Coordinates? entry = null;
        for (int i = 0; i <= size; i++)
        {
            var cell = (i < size) 
                ? Cells[GetIndex(position)] 
                : CellType.Empty;
            
            if (cell == CellType.Empty 
                && entry.HasValue)
            {
                WarpGates[direction].Add(position, entry.Value);
                WarpGates[oppositeDirection].Add(entry.Value + oppositeDirection, position + oppositeDirection);
                entry = null;
            }
            
            if (cell != CellType.Empty 
                && !entry.HasValue)
            {
                entry = position;
            }

            position += direction;
        }
    }

    private void FindStart()
    {
        for (int x = 0; x < Width; x++)
        {
            if (Cells[x] == CellType.Free)
            {
                Start = new Coordinates(x, 0);
                break;
            }
        }
    }

    private int GetIndex(Coordinates coordinates)
    {
        return coordinates.Y * Width + coordinates.X;
    }
}
