namespace Day20GrovePositioningSystem;

internal sealed class Mixer
{
    private static readonly int NodeCapacity = 256;

    public Mixer(
        IEnumerable<int> numbers, 
        int multiplier = 1)
    {
        Zero = null!;
        NumbersByMixingOrder = new();
        Tree = new LeafNode<NumberData>(NodeCapacity);

        int mixingOrder = 0;
        foreach (var number in numbers)
        {
            var numberData = new NumberData(
                mixingOrder, 
                ((long)number) * multiplier);
            
            Tree = Tree.Add(numberData);
            NumbersByMixingOrder[mixingOrder] = numberData;
            mixingOrder++;
            if (number == 0)
            {
                Zero = numberData;
            }
        }
    }

    public NumberData Zero { get; }
    public Dictionary<int, NumberData> NumbersByMixingOrder { get; }
    public Node<NumberData> Tree { get; private set; }

    public long GetCoordinateSum()
    {
        int indexOfZero = Zero.GetIndex();
        long x = Tree[WrapAround(indexOfZero + 1000)].Number;
        long y = Tree[WrapAround(indexOfZero + 2000)].Number;
        long z = Tree[WrapAround(indexOfZero + 3000)].Number;
        return x + y + z;
    }

    public void Mix(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            for (int mixingOrder = 0; mixingOrder < Tree.Count; mixingOrder++)
            {
                var nextToMove = NumbersByMixingOrder[mixingOrder];
                int index = nextToMove.GetIndex();
                Tree.Remove(index);
                int moveToIndex = WrapAround(index + nextToMove.Number);
                Tree.Insert(moveToIndex, nextToMove);
            }
        }
    }

    private int WrapAround(long index)
    {
        int count = Tree.Count;
        return ((int)(index % count) + count) % count;
    }
}

