using Day17PyroclasticFlow;

string inputText = File.ReadAllText("input.txt");
var chamber = new Chamber();
chamber.LoadJets(inputText);
chamber.Simulate(2022);
Console.WriteLine($"The height of the tower after 2022 rocks is: {chamber.TowerHeight}");
chamber.FindPeriod();
long targetRockCount = 1000000000000;
var period = chamber.Period!;
long skippedPeriodCount = (targetRockCount - chamber.RockCount) / period.RockCountDifference!.Value;
long skippedRockCount = skippedPeriodCount * period.RockCountDifference.Value;
int remainingRockCount = (int)(targetRockCount - (chamber.RockCount + skippedRockCount));
chamber.Simulate(remainingRockCount);
long skippedTowerHeight = skippedPeriodCount * period.TowerHeightDifference!.Value;
long finalTowerHeight = skippedTowerHeight + chamber.TowerHeight;
Console.WriteLine($"Tower height after {targetRockCount} rocks is: {finalTowerHeight}");



