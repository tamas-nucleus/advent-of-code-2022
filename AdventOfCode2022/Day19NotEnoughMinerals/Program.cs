using Day19NotEnoughMinerals;

var blueprints = Blueprint.Load("input.txt");
var maxGeodeCounts24 = Calculator.CalculateMaxGeodeCounts(24, blueprints);
int sumQualityLevels = 0;
foreach ((int blueprintId, int geodeCount) in maxGeodeCounts24)
{
    int qualityLevel = blueprintId * geodeCount;
    sumQualityLevels += qualityLevel;
    Console.WriteLine($"With blueprint {blueprintId}, {geodeCount} geodes can be opened, giving a quality level of {qualityLevel}.");
}
Console.WriteLine($"The sum of all quality levels is {sumQualityLevels}.");

var remainingBlueprints = blueprints.Take(3);
var maxGeodeCounts32 = Calculator.CalculateMaxGeodeCounts(32, remainingBlueprints);
int geodeCountsMultiplied = maxGeodeCounts32.Values.Aggregate(1, (a, c) => a * c);
Console.WriteLine($"Max geode counts multiplied for the remaining blueprints: {geodeCountsMultiplied}");
