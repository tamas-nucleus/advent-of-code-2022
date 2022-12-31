namespace Day22MonkeyMap;

internal record TurnInstruction(
    HandDirection Direction
) : Instruction
{
    public override string ToString()
    {
        return $"Turn {Direction}.";
    }
}
