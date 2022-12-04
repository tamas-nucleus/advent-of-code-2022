namespace Day4CampCleanup;

internal readonly record struct SectionRange(
    int Start,
    int End)
{
    public bool Contains(SectionRange other)
    {
        return Start <= other.Start && End >= other.End;
    }

    public bool Overlaps(SectionRange other)
    {
        return End >= other.Start && Start <= other.End;
    }

    public static SectionRange Parse(ReadOnlySpan<char> input)
    {
        int dashIndex = input.IndexOf('-');
        return new SectionRange(
            int.Parse(input.Slice(0, dashIndex)),
            int.Parse(input.Slice(dashIndex + 1)));
    }
}
