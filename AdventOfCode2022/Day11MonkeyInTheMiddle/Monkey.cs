namespace Day11MonkeyInTheMiddle;

internal sealed record Monkey(
    int MonkeyNumber,
    char Operation,
    int? OperationArgument,
    int TestArgument,
    int TrueMonkeyNumber,
    int FalseMonkeyNumber,
    int InspectionEndReliefDivisor)
{
    public Queue<long> ItemQueue { get; } = new Queue<long>();
    public Monkey? TrueMonkey { get; private set; }
    public Monkey? FalseMonkey { get; private set; }
    public int InspectionCount { get; private set; }
    public long? Modulo { get; private set; }

    public static Monkey Parse(ReadOnlySpan<string> lines, int inspectionEndReliefDivisor)
    {
        string operationArgumentString = lines[2]["  Operation: new = old * ".Length..];
        var monkey = new Monkey(
            MonkeyNumber: int.Parse(lines[0]["Monkey ".Length..^1]),
            Operation: lines[2]["  Operation: new = old ".Length],
            OperationArgument: operationArgumentString == "old" ? null : int.Parse(operationArgumentString),
            TestArgument: int.Parse(lines[3]["  Test: divisible by ".Length..]),
            TrueMonkeyNumber: int.Parse(lines[4]["    If true: throw to monkey ".Length..]),
            FalseMonkeyNumber: int.Parse(lines[5]["    If false: throw to monkey ".Length..]),
            InspectionEndReliefDivisor: inspectionEndReliefDivisor);
        var itemList = lines[1]["  Starting items: ".Length..]
            .Split(", ")
            .Select(s => long.Parse(s));
        foreach (var item in itemList)
        {
            monkey.ItemQueue.Enqueue(item);
        }
        return monkey;
    }

    public static Monkey Copy(Monkey other, int inspectionEndReliefDivisor)
    {
        var copy = new Monkey(
            other.MonkeyNumber,
            other.Operation,
            other.OperationArgument,
            other.TestArgument,
            other.TrueMonkeyNumber,
            other.FalseMonkeyNumber,
            inspectionEndReliefDivisor);
        foreach (var item in other.ItemQueue)
        {
            copy.ItemQueue.Enqueue(item);
        }
        return copy;
    }

    public void Initialize(IReadOnlyList<Monkey> monkeyList, long? modulo = null)
    {
        TrueMonkey = monkeyList[TrueMonkeyNumber];
        FalseMonkey = monkeyList[FalseMonkeyNumber];
        Modulo = modulo;
    }

    public void TakeTurn()
    {
        while (ItemQueue.Count > 0)
        {
            InspectItem(ItemQueue.Dequeue());
        }
    }
    
    private void InspectItem(long item)
    {
        long actualOperationArgument = OperationArgument ?? item;
        item = Operation switch
        {
            '+' => item + actualOperationArgument,
            '*' => item * actualOperationArgument,
            _ => throw new InvalidOperationException()
        };
        item /= InspectionEndReliefDivisor;
        if (Modulo.HasValue)
        {
            item %= Modulo.Value;
        }
        var targetMonkey = (item % TestArgument) == 0
            ? TrueMonkey
            : FalseMonkey;
        targetMonkey!.ItemQueue.Enqueue(item);
        InspectionCount++;
    }
}
