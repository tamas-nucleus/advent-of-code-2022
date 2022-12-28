namespace Day18BoilingBoulders;

internal record struct Coordinates(
    int X,
    int Y,
    int Z)
{
    public IEnumerable<Coordinates> GetNeighbours()
    {
        yield return new Coordinates(X + 1, Y, Z);
        yield return new Coordinates(X - 1, Y, Z);
        yield return new Coordinates(X, Y + 1, Z);
        yield return new Coordinates(X, Y - 1, Z);
        yield return new Coordinates(X, Y, Z + 1);
        yield return new Coordinates(X, Y, Z - 1);
    }

    public static bool TryParse(string input, out Coordinates coordinates)
    {
        var parts = input.Split(',');
        if (parts.Length != 3)
        {
            coordinates = default;
            return false;
        }

        coordinates = new Coordinates(
            int.Parse(parts[0]),
            int.Parse(parts[1]),
            int.Parse(parts[2]));
        return true;
    }
}
