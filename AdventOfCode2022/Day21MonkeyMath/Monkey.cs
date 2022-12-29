using System.Text;

namespace Day21MonkeyMath;

internal sealed class Monkey
{
    public Monkey(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public long? Result { get; set; }
    public string? Operand1MonkeyName { get; private init; }
    public string? Operand2MonkeyName { get; private init; }
    public char? Operation { get; private init; }
    public Monkey Operand1Monkey { get; set; } = null!;
    public Monkey Operand2Monkey { get; set; } = null!;

    public static Monkey Parse(string input)
    {
        string monkeyName = input[..4];
        if (int.TryParse(input.AsSpan(6), out int result))
        {
            return new Monkey(monkeyName)
            {
                Result = result
            };
        }
        else
        {
            return new Monkey(monkeyName)
            {
                Operand1MonkeyName = input.Substring(6, 4),
                Operation = input[11],
                Operand2MonkeyName = input.Substring(13, 4)
            };
        }
    }

    public bool TryCalculate()
    {
        if (Result.HasValue)
        {
            return true;
        }

        if (!Operation.HasValue)
        {
            return false;
        }

        if (!Operand1Monkey.Result.HasValue
            || !Operand2Monkey.Result.HasValue)
        {
            return false;
        }

        long operand1 = Operand1Monkey.Result.Value;
        long operand2 = Operand2Monkey.Result.Value;
        Result = Operation!.Value switch
        {
            '+' => operand1 + operand2,
            '-' => operand1 - operand2,
            '*' => operand1 * operand2,
            _ => operand1 / operand2
        };
        return true;
    }

    public override string ToString()
    {
        var builder = new StringBuilder($"{Name}: ");
        string resultString = Result.HasValue ? Result.Value.ToString() : "?";
        if (Operation.HasValue)
        {
            builder.Append(Operand1MonkeyName);
            if (Operand1Monkey.Result.HasValue)
            {
                builder.Append($"(={Operand1Monkey.Result.Value})");
            }
            builder.Append($" {Operation} ");
            builder.Append(Operand2MonkeyName);
            if (Operand2Monkey.Result.HasValue)
            {
                builder.Append($"(={Operand2Monkey.Result.Value})");
            }
            builder.Append($" = {resultString}");
        }
        else
        {
            builder.Append(resultString);
        }
        return builder.ToString();
    }
}
