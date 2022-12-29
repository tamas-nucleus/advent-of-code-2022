namespace Day19NotEnoughMinerals;

internal static class Calculator
{
    private static readonly int ResourceTypeCount = Enum.GetValues<Resource>().Length;
    
    public static Dictionary<int, int> CalculateMaxGeodeCounts(
        int timeLimit, 
        IEnumerable<Blueprint> blueprints)
    {
        var result = new Dictionary<int, int>();
        Parallel.ForEach(blueprints, blueprint =>
        {
            var calculator = new MaxGeodeCountCalculator(blueprint, timeLimit);
            calculator.Calculate();
            result[blueprint.Id] = calculator.MaxGeodeCount;
            Console.WriteLine($"Blueprint {blueprint.Id} calculated using {calculator.IterationCount} iterations.");
        });
        return result;
    }

    private class MaxGeodeCountCalculator
    {
        public MaxGeodeCountCalculator(
            Blueprint blueprint,
            int timeLimit)
        {
            Blueprint = blueprint;
            TimeLimit = timeLimit;
            
            SetMaxCosts();
        }

        public Blueprint Blueprint { get; }
        public int TimeLimit { get; }
        public ResourceList MaxSingleResourceRobotCosts { get; private set; }
        public int IterationCount { get; private set; }
        public ushort MaxGeodeCount { get; private set; }

        public void Calculate()
        {
            CalculateRec(
                new ResourceList(Ore: 1),
                new ResourceList(),
                TimeLimit);
        }

        private void CalculateRec(
            ResourceList income,
            ResourceList treasury,
            int timeLeft)
        {
            if (timeLeft <= 1)
            {
                if (timeLeft == 1)
                {
                    treasury += income;
                }

                if (treasury.Geode > MaxGeodeCount)
                {
                    MaxGeodeCount = treasury.Geode;
                }
                return;
            }

            if (timeLeft < TimeLimit / 3)
            {
                int maxExtraGeodeGainLeft = ((timeLeft - 1) * (timeLeft - 1 + 1)) / 2;
                int currentGeodesByEnd = treasury.Geode + income.Geode * timeLeft;
                int upperLimitMaxGeodeCount = maxExtraGeodeGainLeft + currentGeodesByEnd;
                if (MaxGeodeCount >= upperLimitMaxGeodeCount)
                {
                    return;
                }
            }

            int mayBuildCount = 0;
            for (int i = 0; i < ResourceTypeCount; i++)
            {
                IterationCount++;

                var resource = (Resource)i;
                if (resource != Resource.Geode)
                {
                    ushort maxCost = MaxSingleResourceRobotCosts[resource];
                    int amountByTheEnd = (timeLeft - 1) * income[resource] + treasury[resource];
                    int amountNeededToContinuouslyBuild = maxCost * (timeLeft - 1);
                    if (amountByTheEnd >= amountNeededToContinuouslyBuild)
                    {
                        continue;
                    }
                }

                var robotCost = Blueprint.GetRobotCost(resource);

                bool canBuild = CanBuild(treasury, robotCost);
                if (!canBuild)
                {
                    continue;
                }
                mayBuildCount++;

                CalculateRec(
                    income.Add(resource, 1),
                    treasury - robotCost + income,
                    timeLeft - 1);
            }

            int mightBuildInFutureCount = 2 + Math.Sign(income.Clay) + Math.Sign(income.Obsidian);
            if (mayBuildCount < mightBuildInFutureCount)
            {
                CalculateRec(
                    income,
                    treasury + income,
                    timeLeft - 1);
            }
        }

        private static bool CanBuild(
            ResourceList treasury,
            ResourceList robotCost)
        {
            return treasury.Ore >= robotCost.Ore
                && treasury.Clay >= robotCost.Clay
                && treasury.Obsidian >= robotCost.Obsidian;
        }

        private void SetMaxCosts()
        {
            for (int i = 0; i < ResourceTypeCount; i++)
            {
                var resource = (Resource)i;
                ushort maxCost = 0;
                for (int j = 0; j < ResourceTypeCount; j++)
                {
                    var robotType = (Resource)j;
                    ushort cost = Blueprint.GetRobotCost(robotType)[resource];
                    maxCost = Math.Max(maxCost, cost);
                }
                MaxSingleResourceRobotCosts = MaxSingleResourceRobotCosts.Set(resource, maxCost);
            }
        }
    }
}
