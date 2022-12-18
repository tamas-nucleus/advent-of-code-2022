using Day11MonkeyInTheMiddle;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLines = File.ReadAllLines(inputFile);
var monkeyListWithRelief = new List<Monkey>();
var monkeyListWithoutRelief = new List<Monkey>();
long modulo = 1;
for (int i = 0; i < inputLines.Length; i++)
{
    string line = inputLines[i];
    if (line.Length == 0)
    {
        continue;
    }
    var monkeyWithRelief = Monkey.Parse(inputLines.AsSpan(i, 6), 3);
    i += 6;
    monkeyListWithRelief.Add(monkeyWithRelief);

    var monkeyWithoutRelief = Monkey.Copy(monkeyWithRelief, 1);
    monkeyListWithoutRelief.Add(monkeyWithoutRelief);
    modulo *= monkeyWithoutRelief.TestArgument;
}
foreach (var monkey in monkeyListWithRelief)
{
    monkey.Initialize(monkeyListWithRelief);
}
foreach (var monkey in monkeyListWithoutRelief)
{
    monkey.Initialize(monkeyListWithoutRelief, modulo);
}
for (int i = 0; i < 20; i++)
{
    foreach (var monkey in monkeyListWithRelief)
    {
        monkey.TakeTurn();
    }
}
for (int i = 0; i < 10000; i++)
{
    foreach (var monkey in monkeyListWithoutRelief)
    {
        monkey.TakeTurn();
    }
}

PrintResults(monkeyListWithRelief);
Console.WriteLine();
PrintResults(monkeyListWithoutRelief);

static void PrintResults(IReadOnlyList<Monkey> monkeyList)
{
    foreach (var monkey in monkeyList)
    {
        PrintInspectionCount(monkey);
    }
    long monkeyBusinessLevel = monkeyList
        .Select(m => (long)m.InspectionCount)
        .OrderByDescending(c => c)
        .Take(2)
        .Aggregate(1l, (a, c) => a * c);
    Console.WriteLine($"The level of monkey business is {monkeyBusinessLevel}.");
}

static void PrintInspectionCount(Monkey monkey)
{
    Console.WriteLine($"Monkey {monkey.MonkeyNumber} inspected items {monkey.InspectionCount} times.");
}