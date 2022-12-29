using System.Text;

namespace Day21MonkeyMath;

internal sealed class Calculator
{
    public List<Monkey> MonkeyList { get; } = new();
    public Monkey Root { get; private set; } = null!;
    public Monkey Humn { get; private set; } = null!;

    public void CalculateHumn()
    {
        Humn.Result = null;
        Resolve();

        long target = GetKnownOperand(Root);
        var current = GetUnknownMonkey(Root);
        while (current != Humn)
        {
            switch (current.Operation!.Value)
            {
                case '+':
                    target -= GetKnownOperand(current);
                    break;
                case '*':
                    target /= GetKnownOperand(current);
                    break;
                case '-':
                    if (current.Operand1Monkey.Result.HasValue)
                    {
                        target = current.Operand1Monkey.Result.Value - target;
                    }
                    else
                    {
                        target += current.Operand2Monkey.Result!.Value;
                    }
                    break;
                case '/':
                    if (current.Operand1Monkey.Result.HasValue)
                    {
                        target = current.Operand1Monkey.Result.Value / target;
                    }
                    else
                    {
                        target *= current.Operand2Monkey.Result!.Value;
                    }
                    break;
            };
            current = GetUnknownMonkey(current);
        }
        current.Result = target;
    }

    public void Resolve()
    {
        var monkeysWithoutResult = MonkeyList
            .Where(m => !m.Result.HasValue)
            .ToHashSet();
        var resolvedNow = new List<Monkey>();
        while (true)
        {
            foreach (var monkey in monkeysWithoutResult)
            {
                if (monkey.TryCalculate())
                {
                    resolvedNow.Add(monkey);
                }
            }

            if (resolvedNow.Count == 0)
            {
                break;
            }

            foreach (var monkey in resolvedNow)
            {
                monkeysWithoutResult.Remove(monkey);
            }
            resolvedNow.Clear();
        }
    }

    public static Calculator Load(string path)
    {
        var calculator = new Calculator();

        var lineArray = File.ReadAllLines(path);
        var monkeyNameMap = new Dictionary<string, Monkey>();
        foreach (var line in lineArray)
        {
            if (line.Length == 0)
            {
                continue;
            }

            var monkey = Monkey.Parse(line);
            calculator.MonkeyList.Add(monkey);
            monkeyNameMap[monkey.Name] = monkey;
        }

        calculator.Root = monkeyNameMap["root"];
        calculator.Humn = monkeyNameMap["humn"];
        foreach (var monkey in calculator.MonkeyList)
        {
            if (monkey.Result.HasValue)
            {
                continue;
            }

            monkey.Operand1Monkey = monkeyNameMap[monkey.Operand1MonkeyName!];
            monkey.Operand2Monkey = monkeyNameMap[monkey.Operand2MonkeyName!];
        }

        return calculator;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var monkey in MonkeyList.OrderBy(m => m.Name))
        {
            builder.AppendLine(monkey.ToString());
        }
        return builder.ToString();
    }

    private static long GetKnownOperand(Monkey monkey) =>
        monkey.Operand1Monkey.Result ?? monkey.Operand2Monkey.Result!.Value;

    private static Monkey GetUnknownMonkey(Monkey monkey) =>
        !monkey.Operand1Monkey.Result.HasValue
            ? monkey.Operand1Monkey
            : monkey.Operand2Monkey;
}
