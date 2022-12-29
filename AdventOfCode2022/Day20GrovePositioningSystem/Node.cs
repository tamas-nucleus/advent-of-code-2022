using System.Collections;

namespace Day20GrovePositioningSystem;

internal abstract class Node<T> : IEnumerable<T>
{
    public BranchNode<T>? Parent { get; internal set; }
    public abstract int Count { get; }
    public abstract T this[int index] { get; }

    public Node<T> Add(T item)
    {
        return Insert(Count, item);
    }

    public abstract Node<T> Insert(int index, T item);
    public abstract T Remove(int index);

    public virtual int IndexOf(T item)
    {
        int i = 0;
        foreach (var candidate in this)
        {
            if (object.Equals(candidate, item))
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    public abstract IEnumerator<T> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return string.Join(", ", this);
    }

    internal int GetStartIndex()
    {
        return Parent != null
            ? Parent.StartIndexOf(this)
            : 0;
    }
}
