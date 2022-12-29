using System.Text.RegularExpressions;

namespace Day19NotEnoughMinerals;

internal sealed record Blueprint(
    int Id,
    ResourceList OreRobotCost,
    ResourceList ClayRobotCost,
    ResourceList ObsidianRobotCost,
    ResourceList GeodeRobotCost)
{
    private static readonly Regex inputRegex = new Regex(
        @"Blueprint (\d+):\s+Each ore robot costs (\d+) ore\.\s+Each clay robot costs (\d+) ore\.\s+Each obsidian robot costs (\d+) ore and (\d+) clay\.\s+Each geode robot costs (\d+) ore and (\d+) obsidian\.", 
        RegexOptions.Compiled);

    public ResourceList GetRobotCost(Resource robotType)
    {
        return robotType switch
        {
            Resource.Ore => OreRobotCost,
            Resource.Clay => ClayRobotCost,
            Resource.Obsidian => ObsidianRobotCost,
            _ => GeodeRobotCost
        };
    }

    public static List<Blueprint> Load(string path)
    {
        var resultList = new List<Blueprint>();
        string input = File.ReadAllText(path);
        var allMatches = inputRegex.Matches(input);
        foreach (Match match in allMatches)
        {
            resultList.Add(new Blueprint(
                Id: int.Parse(match.Groups[1].Value),
                OreRobotCost: new ResourceList(
                    Ore: ushort.Parse(match.Groups[2].Value)),
                ClayRobotCost: new ResourceList(
                    Ore: ushort.Parse(match.Groups[3].Value)),
                ObsidianRobotCost: new ResourceList(
                    Ore: ushort.Parse(match.Groups[4].Value),
                    Clay: ushort.Parse(match.Groups[5].Value)),
                GeodeRobotCost: new ResourceList(
                    Ore: ushort.Parse(match.Groups[6].Value),
                    Obsidian: ushort.Parse(match.Groups[7].Value))));
        }
        return resultList;
    }

    public override string ToString()
    {
        return $"Blueprint {Id}: " +
            $"Each ore robot costs {OreRobotCost[Resource.Ore]} ore. " +
            $"Each clay robot costs {ClayRobotCost[Resource.Ore]} ore. " +
            $"Each obsidian robot costs {ObsidianRobotCost[Resource.Ore]} ore and {ObsidianRobotCost[Resource.Clay]} clay. " +
            $"Each geode robot costs {GeodeRobotCost[Resource.Ore]} ore and {GeodeRobotCost[Resource.Obsidian]} obsidian.";
    }
}
