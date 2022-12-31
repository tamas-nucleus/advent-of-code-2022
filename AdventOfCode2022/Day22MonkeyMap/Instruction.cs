namespace Day22MonkeyMap;

internal record Instruction
{
    private static readonly char[] turnLetterArray = new char[] { 'L', 'R' };

    public static List<Instruction> Parse(string input)
    {
        var result = new List<Instruction>(input.Length);
        int index = 0;
        while (index < input.Length)
        {
            int turnIndex = input.IndexOfAny(turnLetterArray, index);
            if (turnIndex == -1)
            {
                if (index < input.Length)
                {
                    result.Add(new MoveInstruction(int.Parse(input.AsSpan(index))));
                }
                break;
            }

            if (turnIndex > index)
            {
                result.Add(new MoveInstruction(int.Parse(input.AsSpan(index, turnIndex - index))));
            }

            result.Add(new TurnInstruction(input[turnIndex] == 'L' ? HandDirection.Left : HandDirection.Right));
            index = turnIndex + 1;
        }
        return result;
    }
}
