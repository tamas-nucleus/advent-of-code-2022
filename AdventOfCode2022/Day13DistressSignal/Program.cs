using Day13DistressSignal;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLineArray = await File.ReadAllLinesAsync(inputFile);
int rightOrderIndexSum = 0;
int pairIndex = 1;
var allPackets = new List<Value>();
for (int i = 0; i < inputLineArray.Length; i += 3)
{
    string firstLine = inputLineArray[i];
    string secondLine = inputLineArray[i + 1];
    var left = Value.Parse(firstLine);
    var right = Value.Parse(secondLine);
    allPackets.Add(left);
    allPackets.Add(right);
    int comparisonResult = left.CompareTo(right);
    if (comparisonResult <= 0)
    {
        rightOrderIndexSum += pairIndex;
    }
    pairIndex++;
}

Console.WriteLine($"Sum of the indices of pairs in right order: {rightOrderIndexSum}");

var dividerArray = new Value[]
{
    Value.Parse("[[2]]"),
    Value.Parse("[[6]]")
};
allPackets.AddRange(dividerArray);
allPackets.Sort();
int dividerIndicesMultiplied = 1;
foreach (var divider in dividerArray)
{
    dividerIndicesMultiplied *= allPackets.IndexOf(divider) + 1;
}
Console.WriteLine($"Divider indices multiplied: {dividerIndicesMultiplied}");

