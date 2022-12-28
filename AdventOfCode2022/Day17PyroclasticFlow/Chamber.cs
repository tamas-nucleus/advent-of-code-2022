using System.Text;

namespace Day17PyroclasticFlow;

internal sealed class Chamber
{
    private static readonly int MaxRockHeight;
    private static readonly Vector RockBirthPosition;
    private static readonly int RequiredEmptySpaceHeight;
    private static readonly int FingerprintHeight;
    public static readonly int Width;

    static Chamber()
    {
        MaxRockHeight = Rock.AllRocks.Max(r => r.Height);
        RockBirthPosition = new Vector(2, 3);
        RequiredEmptySpaceHeight = RockBirthPosition.Dy + MaxRockHeight + 1;
        Width = 7;
        FingerprintHeight = sizeof(ulong) * 8 / Width;
    }

    private int nextRockIndex;
    private int nextJetIndex;

    public Chamber()
    {
        Reset();
    }

    public List<int> JetList { get; } = new List<int>();
    public List<Row> Rows { get; } = new();
    public int FirstEmptyRow { get; private set; }
    public int TowerHeight => FirstEmptyRow - 1;
    public List<Coordinates> RockPositionList { get; } = new();
    public List<int> PossiblePeriodEnds { get; } = new();
    public int FingerprintTopRow { get; private set; }
    public ulong Fingerprint { get; private set; }
    public Dictionary<ulong, int> FingerprintRegister { get; } = new();
    public Dictionary<int, PeriodData> PotentialPeriodDataRegister { get; } = new();
    public PeriodData? Period { get; private set; }
    public int RockCount { get; private set; }

    public void Reset()
    {
        Rows.Clear();
        var bottomRow = new Row(byte.MaxValue >> 1);
        Rows.Add(bottomRow);
        FirstEmptyRow = 1;
        nextRockIndex = 0;
        nextJetIndex = 0;
        RockPositionList.Clear();
        PossiblePeriodEnds.Clear();
        Fingerprint = 0;
        FingerprintRegister.Clear();
        PotentialPeriodDataRegister.Clear();
        Period = null;
        RockCount = 0;
    }

    public void Simulate(int rockCount)
    {
        for (int i = 0; i < rockCount; i++)
        {
            SimulateRock();
        }
    }

    public void FindPeriod()
    {
        Reset();
        while (true)
        {
            SimulateRock();
            RockCount++;

            int inputState = (nextJetIndex + 1) * (nextRockIndex + 1);
            if (FingerprintRegister.TryGetValue(Fingerprint, out int previousInputState))
            {
                if (inputState == previousInputState)
                {
                    if (!PotentialPeriodDataRegister.TryGetValue(inputState, out var potentialPeriod))
                    {
                        potentialPeriod = new PeriodData(Fingerprint, inputState);
                        PotentialPeriodDataRegister.Add(inputState, potentialPeriod);
                    }

                    potentialPeriod.AddNewReadings(RockCount, TowerHeight);
                    if (potentialPeriod.IsRejected)
                    {
                        continue;
                    }

                    if (potentialPeriod.RockCountList.Count >= 10)
                    {
                        Period = potentialPeriod;
                        return;
                    }
                }
            }

            FingerprintRegister[Fingerprint] = inputState;
        }
    }

    public void LoadJets(string input)
    {
        JetList.Clear();
        JetList.AddRange(input
            .Where(c => c == '<' || c == '>')
            .Select(c => c == '<' ? -1 : 1));
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        for (int y = Rows.Count - 1; y > 0; y--)
        {
            var row = Rows[y];
            builder.Append('|');
            for (int x = 0; x < Width; x++)
            {
                builder.Append(row.Get(x) ? '#' : '.');
            }
            builder.AppendLine("|");
        }
        builder.Append('+');
        for (int x = 0; x < Width; x++)
        {
            builder.Append('-');
        }
        builder.AppendLine("+");
        return builder.ToString();
    }

    private void SimulateRock()
    {
        EnsureSpaceAbove();

        var rock = GetNextRock();
        var current = new Coordinates(
            RockBirthPosition.Dx, 
            FirstEmptyRow + RockBirthPosition.Dy);
        while (true)
        {
            int jet = GetNextJet();
            var next = new Coordinates(
                Math.Min(
                    Math.Max(
                        0,
                        current.X + jet),
                    Width - rock.Width),
                current.Y);
            if (CanMoveTo(rock, next))
            {
                current = next;
            }

            next = new Coordinates(current.X, current.Y - 1);
            if (CanMoveTo(rock, next))
            {
                current = next;
                continue;
            }

            Land(rock, current);
            return;
        }
    }

    private void Land(
        Rock rock, 
        Coordinates position)
    {
        var allPoints = GetPoints(rock, position);
        foreach (var point in allPoints)
        {
            SetRock(point);
        }

        int topRockRow = position.Y + rock.Height - 1;
        int towerHeightGain = Math.Max(0, topRockRow + 1 - FirstEmptyRow);
        FirstEmptyRow += towerHeightGain;
        UpdateFingerprint(position.Y, topRockRow);
    }

    private void UpdateFingerprint(
        int bottomRow, 
        int topRow)
    {
        int fingerprintBottomRow = FingerprintTopRow - FingerprintHeight + 1;
        
        int setFromRow = Math.Max(bottomRow, fingerprintBottomRow);
        int setToRow = Math.Min(topRow, FingerprintTopRow);
        for (int rowIndex = setFromRow; rowIndex <= setToRow; rowIndex++)
        {
            var row = Rows[rowIndex];
            int shift = (FingerprintTopRow - rowIndex) * Width;
            Fingerprint &= ~(0b01111111ul << shift);
            Fingerprint |= (ulong)row.Bits << shift;
        }

        for (int rowIndex = FingerprintTopRow + 1; rowIndex <= topRow; rowIndex++)
        {
            var row = Rows[rowIndex];
            Fingerprint <<= Width;
            Fingerprint |= row.Bits;
            FingerprintTopRow++;
        }
    }

    private bool IsAir(Coordinates coordinates)
    {
        var row = Rows[coordinates.Y];
        return !row.Get(coordinates.X);
    }

    private void SetRock(Coordinates coordinates)
    {
        var row = Rows[coordinates.Y];
        Rows[coordinates.Y] = row.SetTrue(coordinates.X);
    }

    private bool CanMoveTo(
        Rock rock,
        Coordinates position)
    {
        var allPoints = GetPoints(rock, position);
        foreach (var point in allPoints)
        {
            if (!IsAir(point))
            {
                return false;
            }
        }
        return true;
    }

    private static IEnumerable<Coordinates> GetPoints(
        Rock rock,
        Coordinates position)
    {
        foreach (var vector in rock.PointVectors)
        {
            yield return position + vector;
        }
    }

    private int GetNextJet()
    {
        int jet = JetList[nextJetIndex];
        nextJetIndex = (nextJetIndex + 1) % JetList.Count;
        return jet;
    }

    private Rock GetNextRock()
    {
        var rock = Rock.AllRocks[nextRockIndex];
        nextRockIndex = (nextRockIndex + 1) % Rock.AllRocks.Count;
        return rock;
    }

    private void EnsureSpaceAbove()
    {
        int toAddCount = RequiredEmptySpaceHeight - (Rows.Count - FirstEmptyRow);
        for (int i = 0; i < toAddCount; i++)
        {
            Rows.Add(new());
        }
    }
}
