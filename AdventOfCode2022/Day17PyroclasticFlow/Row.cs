namespace Day17PyroclasticFlow;

internal struct Row
{
    public Row(int data)
        : this((byte)data)
    {
    }

    public byte Bits { get; set; }

    public Row(byte data)
    {
        this.Bits = data;
    }

    public bool Get(int index)
    {
        return (Bits & (1 << index)) != 0;
    }

    public Row SetTrue(int index)
    {
        return new Row(Bits | (byte)(1 << index));
    }

    public Row SetFalse(int index)
    {
        return new Row(Bits & (byte)~(1 << index));
    }
}
