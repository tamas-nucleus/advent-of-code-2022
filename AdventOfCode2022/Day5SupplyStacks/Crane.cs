namespace Day5SupplyStacks;

internal abstract class Crane
{
    public abstract string CraneType { get; }

    public abstract void Move(List<char> from, List<char> to, int count);
}
