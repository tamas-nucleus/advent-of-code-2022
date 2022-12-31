using System.Text;

namespace Day23UnstableDiffusion;

internal sealed class Map
{
    public Map(IEnumerable<Elf> elves)
    {
        Elves = elves.ToList();
        ElfMap = new();
        foreach (var elf in Elves)
        {
            ElfMap.Add(elf.Position, elf);
        }

        DirectionQueue = new Queue<Vector[]>(new Vector[][]
        {
            new Vector[] { Vector.North, Vector.NorthEast, Vector.NorthWest },
            new Vector[] { Vector.South, Vector.SouthEast, Vector.SouthWest },
            new Vector[] { Vector.West, Vector.NorthWest, Vector.SouthWest },
            new Vector[] { Vector.East, Vector.NorthEast, Vector.SouthEast }
        });
    }

    public IReadOnlyList<Elf> Elves { get; }
    private Dictionary<Vector, Elf> ElfMap { get; }
    private Queue<Vector[]> DirectionQueue { get; }
    public bool PrintDebugMessages { get; set; }
    public int SpreadCount { get; private set; }

    public int GetEmptyTileCount()
    {
        var rectangle = GetRectangle();
        int width = rectangle.BottomRight.X - rectangle.TopLeft.X + 1;
        int height = rectangle.BottomRight.Y - rectangle.TopLeft.Y + 1;
        int area = width * height;
        return area - Elves.Count;
    }

    public void Spread(int turnCount = int.MaxValue)
    {
        if (PrintDebugMessages)
        {
            Console.WriteLine("== Initial State ==");
            Console.WriteLine(this);
        }

        var proposals = new Dictionary<Vector, Elf>();
        for (int turn = 0; turn < turnCount; turn++)
        {
            proposals.Clear();
            bool someoneNeedsToMove = false;
            foreach (var elf in Elves)
            {
                elf.Proposed = null;

                var elfPosition = elf.Position;
                bool hasCloseByElf = false;
                foreach (var direction in Vector.AllDirections)
                {
                    if (ElfMap.ContainsKey(elfPosition + direction))
                    {
                        hasCloseByElf = true;
                        break;
                    }
                }
                if (!hasCloseByElf)
                {
                    continue;
                }
                someoneNeedsToMove = true;

                foreach (var directionGroup in DirectionQueue)
                {
                    bool isAllFree = true;
                    foreach (var direction in directionGroup)
                    {
                        var toCheck = elfPosition + direction;
                        if (ElfMap.ContainsKey(toCheck))
                        {
                            isAllFree = false;
                            break;
                        }
                    }
                    if (!isAllFree)
                    {
                        continue;
                    }

                    var proposed = elfPosition + directionGroup[0];
                    if (proposals.TryGetValue(proposed, out var otherElf))
                    {
                        otherElf.Proposed = null;
                    }
                    else
                    {
                        elf.Proposed = proposed;
                        proposals[proposed] = elf;
                    }
                    break;
                }
            }

            if (!someoneNeedsToMove)
            {
                if (PrintDebugMessages)
                {
                    Console.WriteLine($"Finished in turn {SpreadCount}.");
                }
                break;
            }

            SpreadCount++;
            DirectionQueue.Enqueue(DirectionQueue.Dequeue());

            foreach (var elf in Elves)
            {
                if (!elf.Proposed.HasValue)
                {
                    continue;
                }

                ElfMap.Remove(elf.Position);
                elf.Position = elf.Proposed.Value;
                ElfMap.Add(elf.Position, elf);
            }

            if (PrintDebugMessages)
            {
                Console.WriteLine($"== End of Round {SpreadCount} ==");
                Console.WriteLine(this);
            }
        }
    }

    public (Vector TopLeft, Vector BottomRight) GetRectangle()
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        foreach (var elf in Elves)
        {
            var position = elf.Position;
            if (position.X < minX) { minX = position.X; }
            if (position.X > maxX) { maxX = position.X; }
            if (position.Y < minY) { minY = position.Y; }
            if (position.Y > maxY) { maxY = position.Y; }
        }
        return (new(minX, minY), new(maxX, maxY));
    }

    public static Map Load(string inputPath)
    {
        var lineArray = File.ReadAllLines(inputPath);
        var elves = new List<Elf>();
        for (int y = 0; y < lineArray.Length; y++)
        {
            string line = lineArray[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] != '#')
                {
                    continue;
                }
                
                elves.Add(new Elf
                {
                    Position = new Vector(x, y)
                });
            }
        }
        return new Map(elves);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        var rectangle = GetRectangle();
        for (int y = rectangle.TopLeft.Y; y <= rectangle.BottomRight.Y; y++)
        {
            for (int x = rectangle.TopLeft.X; x <= rectangle.BottomRight.X; x++)
            {
                var position = new Vector(x, y);
                bool isElfThere = ElfMap.ContainsKey(position);
                char toPrint = isElfThere ? '#' : '.';
                builder.Append(toPrint);
            }
            builder.AppendLine();
        }
        return builder.ToString();
    }
}
