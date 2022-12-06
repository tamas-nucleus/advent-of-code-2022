using System.Text.RegularExpressions;
using Day5SupplyStacks;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLineArray = File.ReadAllLines(inputFile);
int lastInitialStacksLineIndex = -1;
for (int i = 0; i < inputLineArray.Length; i++)
{
    string line = inputLineArray[i];
    if (line.Length == 0)
    {
        lastInitialStacksLineIndex = i - 2;
        break;
    }
}

var stacks9000 = new Stacks(CrateMover9000.Default);
for (int i = lastInitialStacksLineIndex; i >= 0; i--)
{
    string line = inputLineArray[i];
    int stackIndex = 0;
    for (int position = 1; position < line.Length; position += 4)
    {
        if (line[position] != ' ')
        {
            char crate = line[position];
            stacks9000.AddCrate(stackIndex, crate);
        }
        stackIndex++;
    }
}

PrintHeading("Initial state");
Console.WriteLine(stacks9000);
Console.WriteLine();

var stacks9001 = new Stacks(CrateMover9001.Default);
stacks9001.CopyFrom(stacks9000);

var moveCommandRegex = new Regex(@"move (\d+) from (\d+) to (\d+)", RegexOptions.Compiled);
for (int i = lastInitialStacksLineIndex + 3; i < inputLineArray.Length; i++)
{
    string line = inputLineArray[i];
    var match = moveCommandRegex.Match(line);
    if (!match.Success)
    {
        continue;
    }

    int count = int.Parse(match.Groups[1].Value);
    int from = int.Parse(match.Groups[2].Value) - 1;
    int to = int.Parse(match.Groups[3].Value) - 1;
    stacks9000.Move(from, to, count);
    stacks9001.Move(from, to, count);
}

PrintAnswer(stacks9000);
PrintAnswer(stacks9001);

void PrintAnswer(Stacks stacks)
{
    PrintHeading($"Final state with {stacks.Crane.CraneType}");
    Console.WriteLine($"Top crates: {stacks.GetTopCrates()}");
    Console.WriteLine();
    Console.WriteLine(stacks9000);
    Console.WriteLine();
}

void PrintHeading(string heading)
{
    Console.WriteLine(heading);
    Console.WriteLine(new string('-', heading.Length));
    Console.WriteLine();
}

