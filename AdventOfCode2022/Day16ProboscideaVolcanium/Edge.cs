namespace Day16ProboscideaVolcanium
{
    internal sealed class Edge
    {
        public Node ToNode { get; set; } = null!;
        public List<ulong> ShortestPathToFlagsInTurns { get; } = new List<ulong>();
        public int Cost { get; set; } = 1;

        public override string ToString()
        {
            return $"{ToNode.Label}-{Cost}";
        }
    }
}
