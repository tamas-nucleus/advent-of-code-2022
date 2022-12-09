using System.Diagnostics;
using Day7NoSpaceLeftOnDevice;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLineArray = File.ReadAllLines(inputFile);
var root = new DeviceDirectory("/");
var currentDirectory = root;
foreach (var line in inputLineArray)
{
    const string CdLinePrefix = "$ cd ";
    const string DirLinePrefix = "dir ";
    if (line.StartsWith(CdLinePrefix))
    {
        string cdTarget = line[CdLinePrefix.Length..];
        currentDirectory = cdTarget switch
        {
            "/" => root,
            ".." => currentDirectory.GetParentDirectoryOrThrow(),
            _ => currentDirectory.GetOrAddSubdirectory(cdTarget)
        };
    }
    else if (line.StartsWith("$"))
    {
    }
    else if (line.StartsWith(DirLinePrefix))
    {
        string subdirectoryName = line[DirLinePrefix.Length..];
        currentDirectory.GetOrAddSubdirectory(subdirectoryName);
    }
    else
    {
        int spaceIndex = line.IndexOf(' ');
        if (spaceIndex != -1)
        {
            long size = long.Parse(line[0..spaceIndex]);
            string fileName = line[(spaceIndex + 1)..];
            currentDirectory.AddFile(fileName, size);
        }
    }
}

const long MaxTotalSize = 100000;
var maxSizeVisitor = new MaxSizeVisitor(MaxTotalSize);
root.CalculateTotalSizesAndVisit(maxSizeVisitor);
Console.WriteLine($"The sum of the total sizes of directories with a total " +
    $"size less than {MaxTotalSize} is {maxSizeVisitor.SumTotalSizes}.");

const long HardDriveSize = 70000000;
const long FreeSpaceNeeded = 30000000;
long spaceAvailable = HardDriveSize - root.LastCalculatedTotalSize;
long needToFreeUp = FreeSpaceNeeded - spaceAvailable;
Debug.Assert(needToFreeUp > 0);

var toDeleteVisitor = new ToDeleteVisitor(needToFreeUp);
root.VisitTopDown(toDeleteVisitor);
Debug.Assert(toDeleteVisitor.DirectoryToDelete != null);
Console.WriteLine($"Directory to delete is {toDeleteVisitor.DirectoryToDelete.Name} with " +
    $"size {toDeleteVisitor.DirectoryToDelete.LastCalculatedTotalSize}.");


