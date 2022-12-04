using Day4CampCleanup;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLineArray = File.ReadAllLines(inputFile);
int fullContainmentPairCount = 0;
int overlapCount = 0;
foreach (var line in inputLineArray)
{
    int commaIndex = line.IndexOf(',');
    if (commaIndex == -1)
    {
        continue;
    }

    var leftRange = SectionRange.Parse(line.AsSpan(0, commaIndex));
    var rightRange = SectionRange.Parse(line.AsSpan(commaIndex + 1));
    if (leftRange.Contains(rightRange) || rightRange.Contains(leftRange))
    {
        fullContainmentPairCount++;
    }

    if (leftRange.Overlaps(rightRange))
    {
        overlapCount++;
    }
}

Console.WriteLine($"Full containment count: {fullContainmentPairCount}");
Console.WriteLine($"Overlap count: {overlapCount}");
