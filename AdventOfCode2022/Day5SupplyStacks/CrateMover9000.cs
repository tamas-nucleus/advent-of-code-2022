namespace Day5SupplyStacks;

internal sealed class CrateMover9000 : Crane
{
    public static readonly CrateMover9000 Default = new();

    public override string CraneType => "CrateMover 9000";

    public override void Move(List<char> from, List<char> to, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int fromStackIndex = from.Count - 1;
            char crate = from[fromStackIndex];
            from.RemoveAt(fromStackIndex);
            to.Add(crate);
        }
    }
}
