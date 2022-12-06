namespace Day5SupplyStacks;

internal sealed class CrateMover9001 : Crane
{
    public static readonly CrateMover9001 Default = new();

    public override string CraneType => "CrateMover 9001";

    public override void Move(List<char> from, List<char> to, int count)
    {
        int firstFromIndexToMove = from.Count - count;
        for (int fromIndex = firstFromIndexToMove; fromIndex < from.Count; fromIndex++)
        {
            char crate = from[fromIndex];
            to.Add(crate);
        }

        from.RemoveRange(firstFromIndexToMove, count);
    }
}
