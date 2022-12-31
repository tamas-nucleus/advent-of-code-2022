using System.Diagnostics;

namespace Day22MonkeyMap;

/// <summary>
/// 3D cube with the origin in the center. Faces of the cube can be rotated and moved
/// so that they are at (X=0,Y=0)-(X=Size,Y=Size). The current face is moved like that,
/// so that we can use natural coordinates matching the input file. When we move off an
/// edge, we find the other face that share that edge point, move that in place and 
/// continue there.
/// </summary>
internal sealed class CubeMap
{
    private static readonly char[] NonEmptyInputCharacters = { '.', '#' };

    public static readonly Matrix Identity = new(
        1, 0, 0,
        0, 1, 0,
        0, 0, 1);
    public static readonly Matrix RotateUp = new(
        1, 0, 0, 
        0, 0, -1, 
        0, 1, 0);
    public static readonly Matrix RotateRight = new(
        0, 0, 1,
        0, 1, 0,
        -1, 0, 0);
    public static readonly Matrix RotateDown = RotateUp.Transpose();
    public static readonly Matrix RotateLeft = RotateRight.Transpose();

    public CubeMap(
        int size,
        IEnumerable<Face> faces,
        IEnumerable<Instruction> instructions)
    {
        Size = size;
        Faces = faces.ToList();
        Top = Faces[0];
        Instructions = instructions.ToList();
    }

    public int Size { get; }
    public Face Top { get; }
    public IReadOnlyList<Face> Faces { get; }
    public IReadOnlyList<Instruction> Instructions { get; }
    public bool PrintDebugMessages { get; set; }

    public int FollowInstructions()
    {
        var face = Top;
        var position = FindStart();
        var direction = Vector.Right;

        if (PrintDebugMessages)
        {
            Console.WriteLine($"Starting at {position} on");
            Console.Write(face);
        }

        foreach (var instruction in Instructions)
        {
            if (PrintDebugMessages)
            {
                Console.WriteLine($"Following instruction: {instruction}");
            }

            if (instruction is TurnInstruction turn)
            {
                direction = direction.Turn(turn.Direction);
            }
            else if (instruction is MoveInstruction move)
            {
                for (int step = 0; step < move.Distance; step++)
                {
                    var next = position + direction;
                    bool hitWall;
                    if (next.X < 0 || next.X >= Size
                        || next.Y < 0 || next.Y >= Size)
                    {
                        var next3d = face.ToGlobal(next);
                        var newFace = Faces
                            .First(f => f != face
                                && f.IsBorderPoint(next3d));
                        next = newFace.ToLocal(next3d).Project2DCoordinates();
                        var newDirection = GetNewDirection(newFace, next);
                        next += newDirection;
                        
                        hitWall = !newFace[next];
                        if (!hitWall)
                        {
                            face = newFace;
                            direction = newDirection;

                            if (PrintDebugMessages)
                            {
                                Console.WriteLine("Changed to");
                                Console.Write(face);
                            }
                        }
                    }
                    else
                    {
                        hitWall = !face[next];
                    }

                    if (hitWall)
                    {
                        if (PrintDebugMessages)
                        {
                            Console.WriteLine("Hit wall.");
                        }
                        break;
                    }
                    position = next;

                    if (PrintDebugMessages)
                    {
                        Console.WriteLine($"Moved to {position}.");
                    }
                }
            }
        }

        int passwordRow = face.InputOffset.Y + position.Y + 1;
        int passwordColumn = face.InputOffset.X + position.X + 1;
        int passwordDirection = GetPasswordDirection(direction);
        return 1000 * passwordRow + 4 * passwordColumn + passwordDirection;
    }

    public static CubeMap Load(string inputPath)
    {
        var lineArray = File.ReadAllLines(inputPath);

        var instructions = Instruction.Parse(lineArray[^1]);

        int totalHeight = lineArray.Length - 2;
        int totalWidth = lineArray.Take(totalHeight).Max(l => l.Length);
        int size = totalHeight == totalWidth
            ? totalHeight / 3
            : Math.Max(totalWidth, totalHeight) / 4;
        var faces = LoadAllFaces(lineArray, totalHeight, size);

        return new CubeMap(size, faces, instructions);
    }

    private static Vector GetNewDirection(
        Face face, 
        Coordinates borderPoint)
    {
        if (borderPoint.X == -1) { return new Vector(1, 0); }
        else if (borderPoint.X == face.Size) { return new Vector(-1, 0); }
        else if (borderPoint.Y == -1) { return new Vector(0, 1); }
        else { return new Vector(0, -1); }
    }

    private Coordinates FindStart()
    {
        var topCells = Top.Cells;
        int x = 0;
        for (; x < Size; x++)
        {
            if (topCells[x])
            {
                break;
            }
        }
        return new Coordinates(x, 0);
    }

    private static int GetPasswordDirection(Vector vector)
    {
        if (vector == Vector.Right) { return 0; }
        else if (vector == Vector.Down) { return 1; }
        else if (vector == Vector.Left) { return 2; }
        else { return 3; }
    }

    private static List<Face> LoadAllFaces(string[] lineArray,
        int totalHeight,
        int size)
    {
        // Top is first.
        var facePaths = FindFaces(lineArray, totalHeight, size);
        var result = new List<Face>();
        foreach (var facePath in facePaths)
        {
            var rotation = Identity;
            for (int i = facePath.Count - 1; i > 0; i--)
            {
                var current = facePath[i];
                var next = facePath[i - 1];
                if (current.X < next.X) { rotation *= RotateLeft; }
                else if (current.X > next.X) { rotation *= RotateRight; }
                else if (current.Y < next.Y) { rotation *= RotateUp; }
                else if (current.Y > next.Y) { rotation *= RotateDown; }
            }
            var faceCoordinates = facePath[^1];
            var face = LoadFace(lineArray, size, faceCoordinates, rotation);
            result.Add(face);
        }
        return result;
    }

    private static List<List<Coordinates>> FindFaces(
        string[] lineArray,
        int totalHeight,
        int size)
    {
        int topStartIndex = lineArray[0].IndexOfAny(NonEmptyInputCharacters);
        var topCoordinates = new Coordinates(topStartIndex, 0);

        var result = new List<List<Coordinates>>();
        var found = new HashSet<Coordinates>();
        var queue = new Queue<List<Coordinates>>();
        queue.Enqueue(new List<Coordinates>() { topCoordinates });
        while (queue.Count > 0)
        {
            var path = queue.Dequeue();
            var coordinates = path[^1];
            if (found.Contains(coordinates))
            {
                continue;
            }
            found.Add(coordinates);
            result.Add(path);

            foreach (var neighbour in GetFaceInputNeighbours(coordinates, size))
            {
                if (found.Contains(neighbour))
                {
                    continue;
                }
                if (neighbour.X < 0 || neighbour.Y < 0 || neighbour.Y >= totalHeight)
                {
                    continue;
                }
                string line = lineArray[neighbour.Y];
                if (neighbour.X >= line.Length
                    || line[neighbour.X] == ' ')
                {
                    continue;
                }

                var newPath = new List<Coordinates>(path);
                newPath.Add(neighbour);
                queue.Enqueue(newPath);
            }
        }

        return result;
    }

    private static IEnumerable<Coordinates> GetFaceInputNeighbours(
        Coordinates coordinates,
        int size)
    {
        yield return new Coordinates(coordinates.X + size, coordinates.Y);
        yield return new Coordinates(coordinates.X, coordinates.Y + size);
        yield return new Coordinates(coordinates.X - size, coordinates.Y);
        yield return new Coordinates(coordinates.X, coordinates.Y - size);
    }

    private static Face LoadFace(
        string[] lineArray,
        int size,
        Coordinates start,
        Matrix rotation)
    {
        var face = new Face(start, size, rotation);
        var cells = face.Cells;
        int cellIndex = 0;
        for (int y = start.Y; y < start.Y + size; y++)
        {
            string line = lineArray[y];
            for (int x = start.X; x < start.X + size; x++)
            {
                char inputChar = line[x];
                Debug.Assert(inputChar != ' ');
                if (inputChar == '.')
                {
                    cells[cellIndex] = true;
                }
                cellIndex++;
            }
        }
        return face;
    }
}
