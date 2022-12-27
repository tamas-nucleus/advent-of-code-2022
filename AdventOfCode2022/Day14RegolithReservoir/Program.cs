using Day14RegolithReservoir;

var map = await Map.Load("input.txt");
map.Simulate();
Console.WriteLine($"Unit of sand at rest before reachinf the abyss {map.RetainedSandAmountReachingTheAbyss}");
Console.WriteLine($"Unit of sand at rest in total {map.RetainedSandAmountTotal}");