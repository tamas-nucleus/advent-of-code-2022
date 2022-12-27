namespace Day14RegolithReservoir;

internal record struct Interval(
    int Start,
    int End) : IComparable<Interval>
{
    public int CompareTo(Interval other)
    {
        return Start.CompareTo(other.Start);
    }

    public bool Contains(int point)
    {
        return point >= Start && point <= End;
    }

    public bool Contains(Interval other)
    {
        return Start <= other.Start && other.End <= End;
    }
}
