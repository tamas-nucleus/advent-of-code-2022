namespace Day7NoSpaceLeftOnDevice;

internal sealed class ToDeleteVisitor : ITopDownDeviceDirectoryVisitor
{
    public ToDeleteVisitor(long minTotalSize)
    {
        MinTotalSize = minTotalSize;
    }

    public long MinTotalSize { get; }
    public DeviceDirectory? DirectoryToDelete { get; private set; }

    public void Visit(
        DeviceDirectory directory, 
        out bool continueDownwards)
    {
        if (directory.LastCalculatedTotalSize < MinTotalSize)
        {
            continueDownwards = false;
            return;
        }

        if (DirectoryToDelete == null 
            || DirectoryToDelete.LastCalculatedTotalSize > directory.LastCalculatedTotalSize)
        {
            DirectoryToDelete = directory;
        }
        continueDownwards = true;
    }
}
