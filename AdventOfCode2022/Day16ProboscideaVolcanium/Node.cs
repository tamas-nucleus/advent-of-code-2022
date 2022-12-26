namespace Day16ProboscideaVolcanium;

internal sealed class Node
{
    public string Label { get; init; } = null!;
    public int Index { get; set; }
    public int FlowRate { get; set; }
    public List<Edge> Edges { get; } = new List<Edge>();

    public List<int> Distances { get; } = new List<int>();
    public List<Node> Paths { get; } = new List<Node>();
    public List<List<Node>> Blockers { get; } = new List<List<Node>>();
    public bool IsPotentialBlocker { get; set; }

    public override string ToString()
    {
        return $"Valve {Label} has flow rate={FlowRate}; tunnels lead to " +
            $"valves {string.Join(", ", Edges)}";
    }
}
