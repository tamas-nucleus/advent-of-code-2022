namespace Day18BoilingBoulders;

internal sealed class Droplet
{
    public HashSet<Coordinates> CubeSet { get; } = new();

    public int Surface { get; private set; }
    public int ExternalSurface { get; private set; }

    public void MeasureSurface()
    {
        Surface = MeasureSurface(CubeSet);

        int minX = CubeSet.Min(c => c.X) - 1;
        int maxX = CubeSet.Max(c => c.X) + 1;
        int minY = CubeSet.Min(c => c.Y) - 1;
        int maxY = CubeSet.Max(c => c.Y) + 1;
        int minZ = CubeSet.Min(c => c.Z) - 1;
        int maxZ = CubeSet.Max(c => c.Z) + 1;

        var inverted = new HashSet<Coordinates>();
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    var cube = new Coordinates(x, y, z);
                    if (!CubeSet.Contains(cube))
                    {
                        inverted.Add(cube);
                    }
                }
            }
        }

        var visited = new HashSet<Coordinates>();
        var queue = new Queue<Coordinates>();
        queue.Enqueue(new Coordinates(minX, minY, minZ));
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!visited.Add(current))
            {
                continue;
            }

            foreach (var neighbour in current.GetNeighbours())
            {
                if (!inverted.Contains(neighbour))
                {
                    continue;
                }

                if (visited.Contains(neighbour))
                {
                    continue;
                }

                if (neighbour.X < minX || neighbour.X > maxX
                    || neighbour.Y < minY || neighbour.Y > maxY
                    || neighbour.Z < minZ || neighbour.Z > maxZ)
                {
                    continue;
                }

                queue.Enqueue(neighbour);
            }
        }

        var pockets = new HashSet<Coordinates>();
        foreach (var cube in inverted)
        {
            if (!visited.Contains(cube))
            {
                pockets.Add(cube);
            }
        }

        ExternalSurface = Surface - MeasureSurface(pockets);
    }

    public void Load(string path)
    {
        var inputLines = File.ReadAllLines(path);
        foreach (var line in inputLines)
        {
            if (Coordinates.TryParse(line, out var cube))
            {
                CubeSet.Add(cube);
            }
        }
    }

    private static int MeasureSurface(HashSet<Coordinates> shape)
    {
        int surface = 0;
        foreach (var cube in shape)
        {
            foreach (var neighbour in cube.GetNeighbours())
            {
                if (!shape.Contains(neighbour))
                {
                    surface++;
                }
            }
        }
        return surface;
    }
}
