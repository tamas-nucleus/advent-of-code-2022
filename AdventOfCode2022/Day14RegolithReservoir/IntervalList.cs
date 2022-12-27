namespace Day14RegolithReservoir;

internal sealed class IntervalList
{
    private readonly List<Interval> list = new();
    private bool isSorted = true;

    public void Add(Interval interval)
    {
        list.Add(interval);
        isSorted = false;
    }

    public bool Contains(int point)
    {
        EnsureSorted();
        int searchResult = list.BinarySearch(
            new Interval(point, point), 
            BinarySearchComparer.Default);
        return searchResult >= 0;
    }

    private void EnsureSorted()
    {
        if (isSorted)
        {
            return;
        }

        list.Sort();
        isSorted = true;
    }

    private sealed class BinarySearchComparer : IComparer<Interval>
    {
        public static readonly BinarySearchComparer Default = new();

        public int Compare(Interval x, Interval y)
        {
            return x.Contains(y) || y.Contains(x)
                ? 0
                : x.CompareTo(y);
        }
    }
}
