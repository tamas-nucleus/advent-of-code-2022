namespace Day20GrovePositioningSystem;

internal sealed record NumberData(
    int MixingOrder,
    long Number
) : INodeAware<NumberData>
{
    private LeafNode<NumberData>? Node { get; set; }
    public int GetIndex() => Node?.IndexOf(this) ?? throw new InvalidOperationException();
    
    public void SetNode(LeafNode<NumberData> node)
    {
        Node = node;
    }
}
