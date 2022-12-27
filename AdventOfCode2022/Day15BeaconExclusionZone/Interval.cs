namespace Day15BeaconExclusionZone;

internal record struct Interval(
    int Start,
    int End) : IComparable<Interval>
{
    public int Length => End - Start + 1;

    public int CompareTo(Interval other)
    {
        return Start.CompareTo(other.Start);
    }

    public bool Overlaps(Interval other)
    {
        return End >= other.Start && other.End >= Start;
    }

    public bool Contains(Interval other)
    {
        return Start <= other.Start && other.End <= End;
    }

    public override string ToString()
    {
        return $"[{Start}-{End}]";
    }
}
