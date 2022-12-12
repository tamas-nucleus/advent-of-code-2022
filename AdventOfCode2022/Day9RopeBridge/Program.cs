using Day9RopeBridge;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLines = File.ReadAllLines(inputFile);
var rope2 = new Rope(2);
var rope10 = new Rope(10);
foreach (string line in inputLines)
{
    if (line.Length == 0)
    {
        continue;
    }

    var direction = line[0] switch
    {
        'U' => Vector.Up,
        'R' => Vector.Right,
        'D' => Vector.Down,
        _ => Vector.Left
    };
    int stepCount = int.Parse(line[2..]);
    rope2.MoveHead(direction, stepCount);
    rope10.MoveHead(direction, stepCount);
}

PrintResult(rope2);
PrintResult(rope10);

static void PrintResult(Rope rope)
{
    Console.WriteLine($"The tail of the {rope.Length} knot rope visited {rope.TailVisitCount} positions.");
}