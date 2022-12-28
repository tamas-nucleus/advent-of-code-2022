namespace Day17PyroclasticFlow;

internal record struct Coordinates(
    int X,
    int Y)
{
    public override string ToString()
    {
        return $"{X},{Y}";
    }
}
