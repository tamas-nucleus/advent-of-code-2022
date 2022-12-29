using System.Diagnostics;

namespace Day20GrovePositioningSystem;

internal sealed class BranchNode<T> : Node<T>
{
    private int count;

    public BranchNode(Node<T> left, Node<T> right)
    {
        Left = left;
        Right = right;

        Left.Parent = this;
        Right.Parent = this;

        count = Left.Count + Right.Count;
    }

    public Node<T> Left { get; internal set; }
    public Node<T> Right { get; internal set; }

    public override int Count => count;

    public override T this[int index]
    {
        get
        {
            if (index < Left.Count)
            {
                return Left[index];
            }
            else
            {
                return Right[index - Left.Count];
            }
        }
    }

    public override Node<T> Insert(int index, T item)
    {
        Debug.Assert(index <= Count);

        count++;
        if (index < Left.Count)
        {
            Left = Left.Insert(index, item);
        }
        else
        {
            Right = Right.Insert(index - Left.Count, item);
        }
        return this;
    }

    public override T Remove(int index)
    {
        Debug.Assert(index < Count);

        count--;
        if (index < Left.Count)
        {
            return Left.Remove(index);
        }
        else
        {
            return Right.Remove(index - Left.Count);
        }
    }

    public override IEnumerator<T> GetEnumerator()
    {
        return Left.Concat(Right).GetEnumerator();
    }

    internal int StartIndexOf(Node<T> child)
    {
        int myStartIndex = GetStartIndex();
        return child == Left
            ? myStartIndex
            : myStartIndex + Left.Count;
    }
}
