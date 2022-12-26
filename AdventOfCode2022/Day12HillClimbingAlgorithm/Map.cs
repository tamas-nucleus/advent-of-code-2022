using System.Collections;

namespace Day12HillClimbingAlgorithm;

internal sealed class Map
{
    private readonly byte[] elevationMap;

    private Map(int width, int height)
    {
        Width = width;
        Height = height;
        elevationMap = new byte[Height * Width];
    }

    public int Width { get; }
    public int Height { get; }
    public Coordinates StartCoordinates { get; private set; }
    public Coordinates TargetCoordinates { get; private set; }

    public int[]? DistancesFromTarget { get; private set; }

    public int BestPathLengthOriginalStart { get; private set; }
    public int BestPathLengthAnyStart { get; private set; }

    public void FindBestPath()
    {
        BestPathLengthOriginalStart = int.MaxValue;
        BestPathLengthAnyStart = int.MaxValue;

        var visited = new BitArray(elevationMap.Length);
        DistancesFromTarget = new int[elevationMap.Length];
        Array.Fill(DistancesFromTarget, int.MaxValue);
        DistancesFromTarget[ToCellIndex(TargetCoordinates)] = 0;
        var queue = new PriorityQueue<Coordinates, int>();
        queue.Enqueue(TargetCoordinates, 0);
        while (queue.Count > 0)
        {
            var coordinates = queue.Dequeue();
            int cellIndex = ToCellIndex(coordinates);
            if (visited[cellIndex])
            {
                continue;
            }
            visited[cellIndex] = true;

            int distanceHere = DistancesFromTarget[cellIndex];
            if (coordinates == StartCoordinates)
            {
                BestPathLengthOriginalStart = distanceHere;
            }

            int elevation = elevationMap[cellIndex];
            if (elevation == 0
                && BestPathLengthAnyStart > distanceHere)
            {
                BestPathLengthAnyStart = distanceHere;
            }

            foreach (var neighbour in GetNeighbours(coordinates))
            {
                int neighbourIndex = ToCellIndex(neighbour);
                if (visited[neighbourIndex])
                {
                    continue;
                }
                
                int elevationDifference = elevation - elevationMap[neighbourIndex];
                if (elevationDifference > 1)
                {
                    continue;
                }
                
                int oldDistance = DistancesFromTarget[neighbourIndex];
                int newDistance = distanceHere + 1;
                if (newDistance >= oldDistance)
                {
                    continue;
                }
                
                DistancesFromTarget[neighbourIndex] = newDistance;
                queue.Enqueue(neighbour, newDistance);
            }
        }
    }

    public static async Task<Map> Load(string path)
    {
        var inputLineArray = (await File.ReadAllLinesAsync(path))
           .Where(l => l.Length > 0)
           .ToList();
        int width = inputLineArray[0].Length;
        int height = inputLineArray.Count;
        var map = new Map(width, height);
        for (int y = 0; y < height; y++)
        {
            string line = inputLineArray[y];
            for (int x = 0; x < width; x++)
            {
                char input = line[x];
                char elevationChar;
                if (input == 'S')
                {
                    elevationChar = 'a';
                    map.StartCoordinates = new Coordinates(x, y);
                }
                else if (input == 'E')
                {
                    elevationChar = 'z';
                    map.TargetCoordinates = new Coordinates(x, y);
                }
                else
                {
                    elevationChar = input;
                }

                int cellIndex = map.ToCellIndex(x, y);
                map.elevationMap[cellIndex] = (byte)(elevationChar - 'a');
            }
        }
        return map;
    }

    private IEnumerable<Coordinates> GetNeighbours(Coordinates coordinates)
    {
        if (coordinates.Y > 0)
        {
            yield return new Coordinates(coordinates.X, coordinates.Y - 1);
        }
        if (coordinates.X < Width - 1)
        {
            yield return new Coordinates(coordinates.X + 1, coordinates.Y);
        }
        if (coordinates.Y < Height - 1)
        {
            yield return new Coordinates(coordinates.X, coordinates.Y + 1);
        }
        if (coordinates.X > 0)
        {
            yield return new Coordinates(coordinates.X - 1, coordinates.Y);
        }
    }

    private int ToCellIndex(int x, int y)
    {
        return Width * y + x;
    }

    private int ToCellIndex(Coordinates coordinates)
    {
        return Width * coordinates.Y + coordinates.X;
    }
}
