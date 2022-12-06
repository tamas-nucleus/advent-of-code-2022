using System.Text;

namespace Day5SupplyStacks;

internal sealed class Stacks
{
    private readonly List<List<char>> stackList = new();

    public Stacks(Crane crane)
    {
        Crane = crane;
    }

    public Crane Crane { get; }

    public void AddCrate(int stackIndex, char crate)
    {
        for (int i = stackList.Count; i < stackIndex + 1; i++)
        {
            stackList.Add(new());
        }

        var stack = stackList[stackIndex];
        stack.Add(crate);
    }

    public void Move(int from, int to, int count)
    {
        var fromStack = stackList[from];
        var toStack = stackList[to];
        Crane.Move(fromStack, toStack, count);
    }

    public string GetTopCrates()
    {
        var result = new StringBuilder(stackList.Count);
        foreach (var stack in stackList)
        {
            char crate = (stack.Count > 0)
                ? stack[^1]
                : ' ';
            result.Append(crate);
        }
        return result.ToString();
    }

    public void CopyFrom(Stacks other)
    {
        stackList.Clear();
        stackList.Capacity = Math.Max(stackList.Capacity, other.stackList.Count);
        for (int i = 0; i < other.stackList.Count; i++)
        {
            stackList.Add(new List<char>(other.stackList[i]));
        }
    }

    public override string ToString()
    {
        if (stackList.Count == 0)
        {
            return string.Empty;
        }

        var lineStack = new Stack<string>();
        string indexLine = string.Join("   ", Enumerable.Range(1, stackList.Count));
        lineStack.Push($" {indexLine} ");

        var stackEnumeratorList = new List<IEnumerator<char>>(stackList.Count);
        foreach (var stack in stackList)
        {
            stackEnumeratorList.Add(stack.GetEnumerator());
        }

        var stringBuilder = new StringBuilder();
        while (true)
        {
            bool isLineEmpty = true;
            stringBuilder.Clear();
            for (int i = 0; i < stackEnumeratorList.Count; i++)
            {
                var enumerator = stackEnumeratorList[i];
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(' ');
                }

                if (!enumerator.MoveNext())
                {
                    stringBuilder.Append("   ");
                }
                else
                {
                    isLineEmpty = false;
                    stringBuilder.Append($"[{enumerator.Current}]");
                }
            }

            if (isLineEmpty)
            {
                break;
            }

            lineStack.Push(stringBuilder.ToString());
        }

        stringBuilder.Clear();
        while (lineStack.Count > 1)
        {
            stringBuilder.AppendLine(lineStack.Pop());
        }

        // There is at least one line with the numbers.
        stringBuilder.Append(lineStack.Pop());

        return stringBuilder.ToString();
    }
}
