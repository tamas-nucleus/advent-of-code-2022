namespace Day14RegolithReservoir;

internal sealed class Map
{
    public static Coordinates SandSource { get; } = new(500, 0);

    public Dictionary<int, IntervalList> Rows { get; } = new();
    public Dictionary<int, IntervalList> Columns { get; } = new();
    public HashSet<Coordinates> Sand { get; } = new();

    public int AbyssStartRowIndex { get; private set; }
    public int GroundRowIndex { get; private set; }

    private bool IsAbyssReached { get; set; }
    public int RetainedSandAmountReachingTheAbyss { get; private set; }
    public int RetainedSandAmountTotal { get; private set; }

    public void Simulate()
    {
        IsAbyssReached = false;
        RetainedSandAmountReachingTheAbyss = 0;
        RetainedSandAmountTotal = 0;
        AbyssStartRowIndex = Rows.Keys.Max() + 1;
        GroundRowIndex = AbyssStartRowIndex + 1;

        while (true)
        {
            var stoppedAt = SimulatUnitOfSand(SandSource);
            if (!stoppedAt.HasValue)
            {
                break;
            }
        }
    }

    public static async Task<Map> Load(string path)
    {
        var map = new Map();
        var inputLineArray = await File.ReadAllLinesAsync(path);
        foreach (var line in inputLineArray)
        {
            if (line.Length == 0)
            {
                continue;
            }

            var pathCoordinates = line.Split(" -> ")
                .Select(p => Coordinates.Parse(p));
            Coordinates? previous = null;
            foreach (var coordinate in pathCoordinates)
            {
                if (previous.HasValue)
                {
                    if (previous.Value.X == coordinate.X)
                    {
                        LoadInterval(map.Columns, coordinate.X, previous.Value.Y, coordinate.Y);
                    }
                    else
                    {
                        LoadInterval(map.Rows, coordinate.Y, previous.Value.X, coordinate.X);
                    }
                }

                previous = coordinate;
            }
        }

        return map;
    }

    private Coordinates? SimulatUnitOfSand(Coordinates start)
    {
        if (!IsAir(start))
        {
            return null;
        }
        
        RetainedSandAmountTotal++;

        var current = start;
        while (true)
        {
            var next = GetNextCoordinatesCandidates(current)
                .Cast<Coordinates?>()
                .FirstOrDefault(c => IsAir(c!.Value));
            if (!next.HasValue)
            {
                Sand.Add(current);
                if (!IsAbyssReached)
                {
                    RetainedSandAmountReachingTheAbyss++;
                }
                return current;
            }            
            if (next.Value.Y >= AbyssStartRowIndex)
            {
                IsAbyssReached = true;
            }
            current = next.Value;
        }
    }

    private static IEnumerable<Coordinates> GetNextCoordinatesCandidates(Coordinates coordinates)
    {
        int newY = coordinates.Y + 1;
        yield return new Coordinates(coordinates.X, newY);
        yield return new Coordinates(coordinates.X - 1, newY);
        yield return new Coordinates(coordinates.X + 1, newY);
    }

    private bool IsAir(Coordinates coordinates)
    {
        return IsAir(Rows, coordinates.Y, coordinates.X)
            && IsAir(Columns, coordinates.X, coordinates.Y)
            && !Sand.Contains(coordinates)
            && coordinates.Y < GroundRowIndex;
    }

    private static bool IsAir(
        Dictionary<int, IntervalList> dictonary,
        int index,
        int point)
    {
        if (dictonary.TryGetValue(index, out var intervalList))
        {
            if (intervalList.Contains(point))
            {
                return false;
            }
        }

        return true;
    }

    private static void LoadInterval(
        Dictionary<int, IntervalList> dictionary,
        int dictionaryKey,
        int point1,
        int point2)
    {
        if (!dictionary.TryGetValue(dictionaryKey, out var intervalList))
        {
            intervalList = new IntervalList();
            dictionary[dictionaryKey] = intervalList;
        }

        var interval = new Interval(
            Math.Min(point1, point2),
            Math.Max(point1, point2));
        intervalList.Add(interval);
    }
}
