using System.Diagnostics;

namespace Day20GrovePositioningSystem;

internal sealed class LeafNode<T> : Node<T>
{
    public LeafNode(int capacity)
    {
        Capacity = capacity;
        ItemList = new List<T>(Capacity);
    }

    public int Capacity { get; }
    public List<T> ItemList { get; }

    public override int Count => ItemList.Count;

    public override T this[int index] => ItemList[index];

    public override Node<T> Insert(int index, T item)
    {
        Debug.Assert(index >= 0 && index <= ItemList.Count);
        
        ItemList.Insert(index, item);
        (item as INodeAware<T>)?.SetNode(this);
        if (ItemList.Count <= Capacity)
        {
            return this;
        }

        var sibling = new LeafNode<T>(Capacity);
        int moveCount = ItemList.Count / 2;
        sibling.ItemList.AddRange(ItemList.Skip(moveCount));
        foreach (var movedItem in sibling.ItemList)
        {
            (movedItem as INodeAware<T>)?.SetNode(sibling);
        }
        ItemList.RemoveRange(moveCount, ItemList.Count - moveCount);
        
        var oldParent = Parent;
        var branchNode = new BranchNode<T>(this, sibling)
        {
            Parent = oldParent
        };
        return branchNode;
    }

    public override T Remove(int index)
    {
        Debug.Assert(index >= 0 && index < ItemList.Count);
        T item = ItemList[index];
        ItemList.RemoveAt(index);
        return item;
    }

    public override int IndexOf(T item)
    {
        int localIndex = ItemList.IndexOf(item);
        if (localIndex < 0)
        {
            return -1;
        }
        return GetStartIndex() + localIndex;
    }

    public override IEnumerator<T> GetEnumerator()
    {
        return ItemList.GetEnumerator();
    }
}
