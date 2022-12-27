using Day15BeaconExclusionZone;

var taskByInputFile = new Dictionary<string, TaskSpecification>()
{
    { "example1.txt", new TaskSpecification(10, 0, 20, 0, 20) },
    { "input.txt", new TaskSpecification(2000000, 0, 4000000, 0, 4000000) }
};

string inputFile = "input.txt";
var map = await Map.Load(inputFile);
var task = taskByInputFile[inputFile];

map.FindLocationsWithoutBeacon(task.TargetRow);
int noBeaconLocationCount = map.LocationsWithoutBeaconInTargetRow.GetSumLength();
Console.WriteLine($"Count of positions where there's no beacon in target row({task.TargetRow}): {noBeaconLocationCount}");

map.FindSendingBeacon(task);
Console.WriteLine($"Location of the beacon sending the distress signal: {map.SendingBeacon}");
Console.WriteLine($"Tuning frequency: {map.TuningFrequency}");