using Day20GrovePositioningSystem;

var inputLines = File.ReadAllLines("input.txt");
var inputNumbers = new List<int>();
foreach (var line in inputLines)
{
    if (int.TryParse(line, out int number))
    {
        inputNumbers.Add(number);
    }
}

var mixer1 = new Mixer(inputNumbers);
mixer1.Mix();
long coordinateSum1 = mixer1.GetCoordinateSum();
Console.WriteLine($"Sum of coordinates mixed wrong: {coordinateSum1}");

var mixer2 = new Mixer(inputNumbers, 811589153);
mixer2.Mix(10);
long coordinateSum2 = mixer2.GetCoordinateSum();
Console.WriteLine($"Sum of coordinates mixed right: {coordinateSum2}");
