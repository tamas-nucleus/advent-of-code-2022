namespace Day7NoSpaceLeftOnDevice;

internal interface ITopDownDeviceDirectoryVisitor
{
    void Visit(
        DeviceDirectory directory, 
        out bool continueDownwards);

}
