using System.Text.RegularExpressions;

namespace Day15BeaconExclusionZone;

internal sealed class Map
{
    private static readonly ulong TuningFrequencyXFactor = 4000000ul;

    private static readonly Regex lineRegex = new(
        @"Sensor at x=([-\d]+), y=([-\d]+): closest beacon is at x=([-\d]+), y=([-\d]+)", 
        RegexOptions.Compiled);

    public List<Sensor> Sensors { get; } = new();

    public List<Coordinates> BeaconsInTargetRow { get; } = new();
    public IntervalList LocationsWithoutBeaconInTargetRow { get; } = new();
    public Coordinates SendingBeacon { get; private set; }
    public ulong TuningFrequency => (ulong)SendingBeacon.X * TuningFrequencyXFactor + (ulong)SendingBeacon.Y;

    public void FindLocationsWithoutBeacon(int targetRow)
    {
        LocationsWithoutBeaconInTargetRow.Clear();
        BeaconsInTargetRow.Clear();
        foreach (var sensor in Sensors)
        {
            if (sensor.ClosestBeacon.Y == targetRow)
            {
                BeaconsInTargetRow.Add(sensor.ClosestBeacon);
            }

            AddCoveredInterval(LocationsWithoutBeaconInTargetRow, sensor, targetRow);
        }

        foreach (var beaconPosition in BeaconsInTargetRow)
        {
            LocationsWithoutBeaconInTargetRow.Remove(beaconPosition.X);
        }
    }

    public void FindSendingBeacon(TaskSpecification task)
    {
        var targetInterval = new Interval(task.MinX, task.MaxX);
        var cover = new IntervalList();
        for (int row = task.MinY; row < task.MaxY; row++)
        {
            cover.Clear();
            foreach (var sensor in Sensors)
            {
                AddCoveredInterval(cover, sensor, row);
            }

            if (!cover.Contains(targetInterval))
            {
                var possibleLocations = targetInterval - cover;
                int beaconX = possibleLocations.First().Start;
                SendingBeacon = new Coordinates(beaconX, row);
                return;
            }
        }
    }

    public static async Task<Map> Load(string path)
    {
        var map = new Map();
        var inputLineArray = await File.ReadAllLinesAsync(path);
        foreach (var line in inputLineArray)
        {
            var match = lineRegex.Match(line);
            if (!match.Success)
            {
                continue;
            }

            var sensor = new Sensor(
                Position: new Coordinates(
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value)),
                ClosestBeacon: new Coordinates(
                    int.Parse(match.Groups[3].Value),
                    int.Parse(match.Groups[4].Value)));
            map.Sensors.Add(sensor);
        }
        return map;
    }

    private void AddCoveredInterval(
        IntervalList intervalList,
        Sensor sensor,
        int row)
    {
        int radius = sensor.BeaconDistance;
        int distance = sensor.Position.GetDistanceFromRow(row);
        int wingSpan = radius - distance;
        if (wingSpan < 0)
        {
            return;
        }

        int sensorColumn = sensor.Position.X;
        intervalList.Add(
            new Interval(
                sensorColumn - wingSpan,
                sensorColumn + wingSpan));
    }
}
