using Day2RockPaperScissors;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLineArray = await File.ReadAllLinesAsync(inputFile);
int wrongScore = 0;
int rightScore = 0;
foreach (var line in inputLineArray)
{
    if (line.Length != 3)
    {
        continue;
    }

    var opponent = (Shape)(((byte)line[0]) - ((byte)'A'));
    
    var wrongMe = (Shape)(((byte)line[2]) - ((byte)'X'));
    var wrongOutcome = (Outcome)((wrongMe - opponent + 3) % 3);
    int wrongShapeScore = 1 + (int)wrongMe;
    int wrongOutcomeScore = ((1 + (int)wrongOutcome) % 3) * 3;
    wrongScore += wrongShapeScore + wrongOutcomeScore;

    var rightOutcome = (Outcome)((((byte)line[2]) - ((byte)'X') + 2) % 3);
    var rightMe = (Shape)(((int)opponent + (int)rightOutcome) % 3);
    int rightShapeScore = 1 + (int)rightMe;
    int rightOutcomeScore = ((1 + (int)rightOutcome) % 3) * 3;
    rightScore += rightShapeScore + rightOutcomeScore;
}

Console.WriteLine($"Wrong total score: {wrongScore}");
Console.WriteLine($"Right total score: {rightScore}");
