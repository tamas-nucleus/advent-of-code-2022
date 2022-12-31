namespace Day22MonkeyMap;

internal record MoveInstruction(
    int Distance
) : Instruction
{
    public override string ToString()
    {
        return $"Go {Distance} forward.";
    }
}
