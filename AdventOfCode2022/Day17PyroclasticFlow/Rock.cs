namespace Day17PyroclasticFlow;

internal sealed class Rock
{
    public static readonly Rock Dash = new(
        nameof(Dash),
        new Vector(0, 0),
        new Vector(1, 0),
        new Vector(2, 0),
        new Vector(3, 0));

    public static readonly Rock Plus = new(
        nameof(Plus),
        new Vector(1, 0),
        new Vector(0, 1),
        new Vector(1, 1),
        new Vector(2, 1),
        new Vector(1, 2));

    public static readonly Rock L = new(
        nameof(L),
        new Vector(0, 0),
        new Vector(1, 0),
        new Vector(2, 0),
        new Vector(2, 1),
        new Vector(2, 2));

    public static readonly Rock I = new(
        nameof(I),
        new Vector(0, 0),
        new Vector(0, 1),
        new Vector(0, 2),
        new Vector(0, 3));

    public static readonly Rock Dot = new(
        nameof(Dot),
        new Vector(0, 0),
        new Vector(0, 1),
        new Vector(1, 0),
        new Vector(1, 1));

    public static readonly IReadOnlyList<Rock> AllRocks = new Rock[] { Dash, Plus, L, I, Dot };

    public Rock(
        string name, 
        params Vector[] pointsVectors)
    {
        Name = name;
        PointVectors = pointsVectors.ToList();

        Height = PointVectors.Max(p => p.Dy) + 1;
        Width = PointVectors.Max(p => p.Dx) + 1;
    }

    public string Name { get; }
    public IReadOnlyList<Vector> PointVectors { get; }
    public int Height { get; }
    public int Width { get; }

    public override string ToString()
    {
        return Name;
    }
}
