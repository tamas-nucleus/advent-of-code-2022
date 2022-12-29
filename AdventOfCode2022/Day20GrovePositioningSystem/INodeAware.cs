namespace Day20GrovePositioningSystem;

internal interface INodeAware<T>
{
    void SetNode(LeafNode<T> node);
}
