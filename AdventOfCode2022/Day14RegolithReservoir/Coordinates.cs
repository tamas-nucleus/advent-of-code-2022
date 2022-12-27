namespace Day14RegolithReservoir;

internal record struct Coordinates(
    int X,
    int Y)
{
    public static Coordinates Parse(string input)
    {
        int index = input.IndexOf(',');
        return new(
            int.Parse(input.AsSpan(0, index)), 
            int.Parse(input.AsSpan(index + 1)));
    }
}
