namespace Day9RopeBridge;

internal sealed class Rope
{
    private readonly Coordinates[] sectionCoordinates;
    private readonly HashSet<Coordinates> tailVisitedCoordinates;

    public Rope(int length)
    {
        sectionCoordinates = new Coordinates[length];
        tailVisitedCoordinates = new HashSet<Coordinates>();
        tailVisitedCoordinates.Add(sectionCoordinates[^1]);
    }

    public int Length => sectionCoordinates.Length;
    public int TailVisitCount => tailVisitedCoordinates.Count;

    public void MoveHead(Vector direction, int count)
    {
        for (int i = 0; i < count; i++)
        {
            MoveHead(direction);
        }
    }

    private void MoveHead(Vector direction)
    {
        var coordinatesAhead = sectionCoordinates[0] += direction;
        for (int i = 1; i < sectionCoordinates.Length; i++)
        {
            var coordinates = sectionCoordinates[i];
            var displacement = coordinatesAhead - coordinates;
            if (displacement.Length <= 1)
            {
                break;
            }
            var move = new Vector(displacement.Dx / 2, displacement.Dy / 2);
            coordinates = coordinatesAhead - move;
            sectionCoordinates[i] = coordinates;
            if (i == sectionCoordinates.Length - 1)
            {
                tailVisitedCoordinates.Add(coordinates);
            }
            coordinatesAhead = coordinates;
        }
    }
}
