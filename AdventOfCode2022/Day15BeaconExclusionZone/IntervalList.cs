using System.Collections;

namespace Day15BeaconExclusionZone;

internal sealed class IntervalList : IEnumerable<Interval>
{
    private readonly List<Interval> list = new();

    public int Count => list.Count;

    public int GetSumLength()
    {
        return list.Sum(i => i.Length);
    }

    public void Clear()
    {
        list.Clear();
    }

    public void Add(Interval interval)
    {
        int index = list.BinarySearch(interval, BinarySearchComparer.Default);
        if (index < 0)
        {
            list.Insert(~index, interval);
            return;
        }

        int overlapStart = index;
        int overlapEnd = index;
        while (overlapStart > 0
            && list[overlapStart - 1].Overlaps(interval))
        {
            overlapStart--;
        }

        while (overlapEnd < list.Count - 1
            && list[overlapEnd + 1].Overlaps(interval))
        {
            overlapEnd++;
        }

        if (overlapStart > 0 
            && list[overlapStart].End == interval.Start - 1)
        {
            overlapStart--;
        }

        if (overlapEnd < list.Count - 1 
            && list[overlapEnd].Start == interval.End + 1)
        {
            overlapEnd++;
        }

        int newStart = Math.Min(list[overlapStart].Start, interval.Start);
        int newEnd = Math.Max(list[overlapEnd].End, interval.End);
        list.RemoveRange(overlapStart, overlapEnd - overlapStart);
        list[overlapStart] = new Interval(newStart, newEnd);
    }

    public void Remove(int point)
    {
        int index = list.BinarySearch(
            new Interval(point, point), 
            BinarySearchComparer.Default);
        if (index < 0)
        {
            return;
        }

        var containingInterval = list[index];
        if (containingInterval.Length == 1)
        {
            list.RemoveAt(index);
            return;
        }

        if (containingInterval.Start == point)
        {
            list[index] = new Interval(containingInterval.Start + 1, containingInterval.End);
        }
        else if (containingInterval.End == point)
        {
            list[index] = new Interval(containingInterval.Start, containingInterval.End - 1);
        }
        else
        {
            list[index] = new Interval(containingInterval.Start, point - 1);
            list.Insert(index + 1, new Interval(point + 1, containingInterval.End));
        }
    }

    public bool Contains(Interval interval)
    {
        int index = list.BinarySearch(interval, BinarySearchComparer.Default);
        if (index < 0)
        {
            return false;
        }

        var overlappingInterval = list[index];
        return overlappingInterval.Contains(interval);
    }

    public static IntervalList operator -(
        Interval interval, 
        IntervalList intervalList)
    {
        var result = new IntervalList();
        var listToSubtract = intervalList.list;
        var resultList = result.list;
        if (listToSubtract.Count == 0)
        {
            result.list.Add(interval);
        }
        else
        {
            var first = listToSubtract[0];
            if (interval.Start < first.Start)
            {
                resultList.Add(new Interval(interval.Start, first.Start - 1));
            }

            var last = listToSubtract[^1];
            if (interval.End > last.End)
            {
                resultList.Add(new Interval(last.End + 1, interval.End));
            }

            var previous = first;
            for (int i = 1; i < listToSubtract.Count; i++)
            {
                var current = listToSubtract[i];
                int start = Math.Max(previous.End + 1, interval.Start);
                int end = Math.Min(current.Start - 1, interval.End);
                if (start <= end)
                {
                    resultList.Add(new(start, end));
                }
                previous = current;
            }
        }
        return result;
    }

    public override string ToString()
    {
        return string.Join(", ", list);
    }

    public IEnumerator<Interval> GetEnumerator()
    {
        return ((IEnumerable<Interval>)list).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)list).GetEnumerator();
    }

    private sealed class BinarySearchComparer : IComparer<Interval>
    {
        public static readonly BinarySearchComparer Default = new();

        public int Compare(Interval x, Interval y)
        {
            return x.Overlaps(y)
                ? 0
                : x.CompareTo(y);
        }
    }
}
