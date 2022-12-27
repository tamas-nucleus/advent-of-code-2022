namespace Day13DistressSignal;

internal sealed class Value : IComparable<Value>
{
    public IReadOnlyList<Value>? ValueList { get; private init; }
    public int? Number { get; private init; }

    public static Value Parse(string input)
    {
        int index = 0;
        return Parse(input, ref index);
    }

    public int CompareTo(Value? other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (Number.HasValue && other.Number.HasValue)
        {
            return Number.Value.CompareTo(other.Number.Value);
        }
        else if (ValueList != null && other.ValueList != null)
        {
            return CompareLists(ValueList, other.ValueList);
        }
        else if (ValueList != null)
        {
            return CompareListWithNumber(ValueList, other);
        }
        else
        {
            return -1 * CompareListWithNumber(other.ValueList!, this);
        }
    }

    public override string ToString()
    {
        if (ValueList != null)
        {
            return $"[{string.Join(',', ValueList)}]";
        }
        else
        {
            return Number!.Value.ToString();
        }
    }

    private static int CompareLists(
        IReadOnlyList<Value> left, 
        IReadOnlyList<Value> right)
    {
        int minLength = Math.Min(left.Count, right.Count);
        for (int i = 0; i < minLength; i++)
        {
            int comparisonResult = left[i].CompareTo(right[i]);
            if (comparisonResult != 0)
            {
                return comparisonResult;
            }
        }

        return left.Count - right.Count;
    }

    private static int CompareListWithNumber(
        IReadOnlyList<Value> left,
        Value right)
    {
        if (left.Count == 0)
        {
            return -1;
        }
        int comparisonResult = left[0].CompareTo(right);
        if (comparisonResult != 0)
        {
            return comparisonResult;
        }
        return left.Count > 1 ? 1 : 0;
    }

    private static Value Parse(string input, ref int index)
    {
        return input[index] switch
        {
            '[' => ParseList(input, ref index),
            _ => ParseNumber(input, ref index)
        };
    }

    private static Value ParseList(string input, ref int index)
    {
        index++;
        var valueList = new List<Value>();
        while (true)
        {
            char nextChar = input[index];
            if (nextChar == ']')
            {
                index++;
                return new Value
                {
                    ValueList = valueList
                };
            }
            else if (nextChar == ',')
            {
                index++;
            }

            var value = Parse(input, ref index);
            valueList.Add(value);
        }
    }

    private static Value ParseNumber(string input, ref int index)
    {
        int startIndex = index;
        while (true)
        {
            char inputChar = input[index];
            if (inputChar < '0' || inputChar > '9')
            {
                break;
            }
            index++;
        }

        int number = int.Parse(input.AsSpan(startIndex, index - startIndex));
        return new Value
        {
            Number = number
        };
    }
}
