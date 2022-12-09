namespace Day7NoSpaceLeftOnDevice;

internal sealed class MaxSizeVisitor : IBottomUpDeviceDirectoryVisitor
{
    public MaxSizeVisitor(long maxTotalSize)
    {
        MaxTotalSize = maxTotalSize;
    }

    public long MaxTotalSize { get; }
    
    public long SumTotalSizes { get; private set; }

    public void Visit(DeviceDirectory directory)
    {
        if (directory.LastCalculatedTotalSize <= MaxTotalSize)
        {
            SumTotalSizes += directory.LastCalculatedTotalSize;
        }
    }
}
