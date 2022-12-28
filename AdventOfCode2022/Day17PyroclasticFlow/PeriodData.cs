using System.Text;

namespace Day17PyroclasticFlow;

internal record PeriodData(
    ulong Fingerprint,
    int InputState)
{
    public List<int> RockCountList { get; } = new();
    public List<int> TowerHeightList { get; } = new();
    public int? RockCountDifference { get; private set; }
    public int? TowerHeightDifference { get; private set; }
    public bool IsRejected { get; set; }

    public void AddNewReadings(int rockCount, int towerHeight)
    {
        RockCountList.Add(rockCount);
        TowerHeightList.Add(towerHeight);
        if (RockCountList.Count == 1)
        {
            return;
        }

        if (RockCountList.Count == 2)
        {
            RockCountDifference = RockCountList[1] - RockCountList[0];
            TowerHeightDifference = TowerHeightList[1] - TowerHeightList[0];
            return;
        }

        for (int i = 2; i < RockCountList.Count; i++)
        {
            int currentRockCountDifference = RockCountList[i] - RockCountList[i - 1];
            int currentTowerHeightDifference = TowerHeightList[i] - TowerHeightList[i - 1];
            if (RockCountDifference != currentRockCountDifference
                || TowerHeightDifference != currentTowerHeightDifference)
            {
                RockCountDifference = null;
                TowerHeightDifference = null;
                IsRejected = true;
            }
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"InputState={InputState}, Fingerprint={Fingerprint}");
        for (int i = 0; i < RockCountList.Count; i++)
        {
            builder.AppendLine($"\tRockCount={RockCountList[i]} (+{RockCountDifference}), " +
                $"TowerHeight={TowerHeightList[i]} (+{TowerHeightDifference})");
        }
        return builder.ToString();
    }
}
