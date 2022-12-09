namespace Day7NoSpaceLeftOnDevice;

internal sealed class DeviceDirectory
{
    private readonly Dictionary<string, DeviceDirectory> subdirectoryMap = new();
    private readonly Dictionary<string, long> fileMap = new();

    public DeviceDirectory(string name)
        : this(name, null)
    {
    }

    private DeviceDirectory(
        string name,
        DeviceDirectory? parentDirectory)
    {
        Name = name;
        ParentDirectory = parentDirectory;
    }

    public DeviceDirectory? ParentDirectory { get; }
    public string Name { get; }
    public long ExclusiveSize { get; private set; }
    public long LastCalculatedTotalSize { get; private set; }

    public void AddFile(string name, long size)
    {
        fileMap.Add(name, size);
        ExclusiveSize += size;
    }

    public DeviceDirectory GetOrAddSubdirectory(string name)
    {
        if (!subdirectoryMap.TryGetValue(name, out var subdirectory))
        {
            subdirectory = new DeviceDirectory(name, this);
            subdirectoryMap.Add(name, subdirectory);
        }
        return subdirectory;
    }

    public DeviceDirectory GetParentDirectoryOrThrow()
    {
        return ParentDirectory 
            ?? throw new InvalidOperationException();
    }

    public void VisitTopDown(ITopDownDeviceDirectoryVisitor visitor)
    {
        var queue = new Queue<DeviceDirectory>();
        queue.Enqueue(this);
        while (queue.Count > 0)
        {
            var directory = queue.Dequeue();
            visitor.Visit(directory, out bool continueDownwards);
            if (continueDownwards)
            {
                foreach (var subdirectory in directory.subdirectoryMap.Values)
                {
                    queue.Enqueue(subdirectory);
                }
            }
        }
    }

    public void CalculateTotalSizesAndVisit(IBottomUpDeviceDirectoryVisitor visitor)
    {
        CalculateTotalSizesAndVisitRec(visitor, this);
    }

    private static void CalculateTotalSizesAndVisitRec(
        IBottomUpDeviceDirectoryVisitor visitor, 
        DeviceDirectory current)
    {
        long totalSize = current.ExclusiveSize;
        foreach (var subdirectory in current.subdirectoryMap.Values)
        {
            CalculateTotalSizesAndVisitRec(visitor, subdirectory);
            totalSize += subdirectory.LastCalculatedTotalSize;
        }
        current.LastCalculatedTotalSize = totalSize;
        visitor.Visit(current);
    }
}
