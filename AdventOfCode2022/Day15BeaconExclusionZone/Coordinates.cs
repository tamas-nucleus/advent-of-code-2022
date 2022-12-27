namespace Day15BeaconExclusionZone;

internal record struct Coordinates(
    int X,
    int Y)
{
    public int GetDistanceFrom(Coordinates other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    public int GetDistanceFromRow(int row)
    {
        return Math.Abs(Y - row);
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}