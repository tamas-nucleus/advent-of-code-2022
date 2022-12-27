namespace Day15BeaconExclusionZone;

internal sealed record Sensor(
    Coordinates Position,
    Coordinates ClosestBeacon)
{
    public int BeaconDistance => Position.GetDistanceFrom(ClosestBeacon);
}
