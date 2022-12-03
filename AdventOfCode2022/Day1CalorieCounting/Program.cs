string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLineArray = await File.ReadAllLinesAsync(inputFile);
var elfList = new List<(int ElfNumber, int CalorieSum)>();
int calorieSum = 0;
int elfNumber = 1;
foreach (var line in inputLineArray)
{
    if (line.Length == 0)
    {
        elfList.Add((elfNumber, calorieSum));
        calorieSum = 0;
        elfNumber++;
        continue;
    }

    calorieSum += int.Parse(line);
}

if (calorieSum > 0)
{
    elfList.Add((elfNumber, calorieSum));
}

var topThree = elfList
    .OrderByDescending(p => p.CalorieSum)
    .Take(3)
    .ToList();
var top = topThree[0];

Console.WriteLine($"Most calories: elf {top.ElfNumber} with {top.CalorieSum} calories.");

int topThreeCalorieSum = topThree.Sum(p => p.CalorieSum);
string topThreeElfNumbers = string.Join(", ", topThree.Select(p => p.ElfNumber));

Console.WriteLine($"Top three: elves {topThreeElfNumbers} carrying {topThreeCalorieSum} calories.");
