string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLines = File.ReadAllLines(inputFile);
int lineIndex = 0;
int cycle = 1;
int spriteX = 1;
int sumSignalStrength = 0;
int nextCommandCycle = 1;
int dSpriteX = 0;
while (true)
{
    if (nextCommandCycle == cycle)
    {
        spriteX += dSpriteX;

        if (lineIndex == inputLines.Length)
        {
            break;
        }
        string line = inputLines[lineIndex++];
        int commandCycleLength;
        if (line == "noop")
        {
            commandCycleLength = 1;
            dSpriteX = 0;
        }
        else if (line.StartsWith("addx"))
        {
            commandCycleLength = 2;
            dSpriteX = int.Parse(line[5..]);
        }
        else
        {
            break;
        }

        nextCommandCycle += commandCycleLength;
    }

    if (cycle % 20 == 0 
        && ((cycle / 20) & 1) == 1)
    {
        sumSignalStrength += cycle * spriteX;
    }

    int zeroBasedCycle = cycle - 1;
    int pixelX = zeroBasedCycle % 40;
    if (pixelX == 0 && zeroBasedCycle != 0)
    {
        Console.WriteLine();
    }

    bool isPixelLit = (spriteX - 1 <= pixelX && spriteX + 1 >= pixelX);
    Console.Write(isPixelLit ? '#' : '.');
    
    cycle++;
}

Console.WriteLine();
Console.WriteLine($"Sum signal strength: {sumSignalStrength}");