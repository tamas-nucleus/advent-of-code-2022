using Day8TreetopTreeHouse;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLines = File.ReadAllLines(inputFile)
    .Where(l => l.Length > 0)
    .ToList();
int width = inputLines[0].Length;
int height = inputLines.Count;
var map = new TreeMap(width, height);
var row = new byte[width];
for (int y = 0; y < inputLines.Count; y++)
{
    string line = inputLines[y];
    for (int x = 0; x < line.Length; x++)
    {
        row[x] = (byte)(line[x] - '0');
        map.SetRow(y, row);
    }
}

int visibleTreeCount = map.CalculateOutsideVisibility();
Console.WriteLine($"Visible tree count: {visibleTreeCount}");

var (bestScenicScoreCoordinates, bestScenicScore) = map.FindMaxScenicScore();
Console.WriteLine($"Best scenic score is {bestScenicScore} at {bestScenicScoreCoordinates}");
