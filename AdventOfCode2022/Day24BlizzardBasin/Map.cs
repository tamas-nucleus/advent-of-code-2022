using System.Text;

namespace Day24BlizzardBasin;

internal sealed class Map
{
    public static readonly IReadOnlyList<Vector> AllActions = new Vector[] 
    { 
        Vector.Zero, Vector.Up, Vector.Right, Vector.Down, Vector.Left
    };

    public Map(
        int width,
        int height,
        IEnumerable<Blizzard> blizzards,
        int[] blizzardCounts,
        Vector entrance,
        Vector exit)
    {
        Width = width;
        Height = height;
        Blizzards = blizzards.ToList();
        BlizzardCounts = (int[])blizzardCounts.Clone();
        Entrance = entrance;
        Exit = exit;
    }

    public int Width { get; }
    public int Height { get; }
    public List<Blizzard> Blizzards { get; }
    public int[] BlizzardCounts { get; }
    public Vector Entrance { get; }
    public Vector Exit { get; }
    public bool PrintDebugMessages { get; set; }

    public int Time { get; private set; }

    public void FindFastestPath(bool reverse = false)
    {
        var start = reverse ? Exit : Entrance;
        var goal = reverse ? Entrance : Exit;

        var currentSet = new HashSet<Vector>();
        currentSet.Add(start);
        var nextSet = new HashSet<Vector>();

        if (PrintDebugMessages)
        {
            Console.WriteLine("Initial state:");
            PrintState(currentSet);
        }

        while (true)
        {
            Time++;
            MoveBlizzards();
            nextSet.Clear();
            foreach (var position in currentSet)
            {
                foreach (var action in AllActions)
                {
                    var next = position + action;
                    if (!IsValid(next))
                    {
                        continue;
                    }
                    if (next == goal)
                    {
                        return;
                    }
                    if (BlizzardCounts[ToIndex(next)] > 0)
                    {
                        continue;
                    }
                    nextSet.Add(next);
                }
            }

            var tempSet = currentSet;
            currentSet = nextSet;
            nextSet = tempSet;

            if (PrintDebugMessages)
            {
                Console.WriteLine($"Minute {Time}, move down:");
                PrintState(currentSet);
            }
        }
    }

    public static Map Load(string inputPath)
    {
        var lineArray = File.ReadAllLines(inputPath);
        int width = lineArray[0].Length;
        int height = lineArray.Length;
        int entranceX = lineArray[0].IndexOf('.');
        int exitX = lineArray[^1].IndexOf('.');
        int index = width;
        var blizzardList = new List<Blizzard>((width - 2) * (height - 2));
        var blizzardCounts = new int[width * height];
        for (int y = 1; y < height - 1; y++)
        {
            string line = lineArray[y];
            index++;
            for (int x = 1; x < width - 1; x++)
            {
                char inputChar = line[x];
                if (inputChar != '.')
                {
                    var blizzard = new Blizzard()
                    {
                        Position = new Vector(x, y),
                        Direction = inputChar switch
                        {
                            '^' => Vector.Up,
                            '>' => Vector.Right,
                            'v' => Vector.Down,
                            _ => Vector.Left,
                        }
                    };
                    blizzardList.Add(blizzard);
                    blizzardCounts[index] = 1;
                }
                index++;
            }
            index++;
        }
        return new Map(
            width,
            height,
            blizzardList,
            blizzardCounts,
            new(entranceX, 0),
            new(exitX, height - 1));
    }

    private void PrintState(HashSet<Vector> possiblePositions)
    {
        var blizzardMap = new Dictionary<Vector, Blizzard>();
        foreach (var blizzard in Blizzards)
        {
            blizzardMap[blizzard.Position] = blizzard;
        }

        var builder = new StringBuilder();
        for (int x = 0; x < Width; x++)
        {
            builder.Append(x == Entrance.X ? '.' : '#');
        }
        builder.AppendLine();
        for (int y = 1; y < Height - 1; y++)
        {
            builder.Append('#');
            for (int x = 1; x < Width - 1; x++)
            {
                var position = new Vector(x, y);
                int blizzardCount = BlizzardCounts[ToIndex(position)];
                char toPrint;
                if (possiblePositions.Contains(position))
                {
                    toPrint = 'o';
                }
                else if (blizzardCount == 0)
                {
                    toPrint = '.';
                }
                else if (blizzardCount > 1)
                {
                    toPrint = blizzardCount.ToString()[0];
                }
                else
                {
                    var blizzard = blizzardMap[position];
                    if (blizzard.Direction == Vector.Up) { toPrint = '^'; }
                    else if (blizzard.Direction == Vector.Right) { toPrint = '>'; }
                    else if (blizzard.Direction == Vector.Down) { toPrint = 'v'; }
                    else { toPrint = '<'; }
                }
                builder.Append(toPrint);
            }
            builder.Append('#');
            builder.AppendLine();
        }
        for (int x = 0; x < Width; x++)
        {
            builder.Append(x == Exit.X ? '.' : '#');
        }
        builder.AppendLine();
        Console.WriteLine(builder);
    }

    private bool IsValid(Vector position)
    {
        return (position == Entrance || position == Exit)
            || (position.X > 0 && position.X < Width - 1
            && position.Y > 0 && position.Y < Height - 1);
    }

    private void MoveBlizzards()
    {
        foreach (var blizzard in Blizzards)
        {
            var position = blizzard.Position;
            BlizzardCounts[ToIndex(position)]--;
            position += blizzard.Direction;
            if (position.X == 0) { position = new(Width - 2, position.Y); }
            else if (position.X == Width - 1) { position = new(1, position.Y); }
            if (position.Y == 0) { position = new(position.X, Height - 2); }
            else if (position.Y == Height - 1) { position = new(position.X, 1); }
            BlizzardCounts[ToIndex(position)]++;
            blizzard.Position = position;
        }
    }

    private int ToIndex(Vector position)
    {
        return position.Y * Width + position.X;
    }
}
