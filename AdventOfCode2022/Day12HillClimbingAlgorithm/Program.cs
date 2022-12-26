using Day12HillClimbingAlgorithm;

var map = await Map.Load("input.txt");
map.FindBestPath();
Console.WriteLine($"Length of the shortest path from start: {map.BestPathLengthOriginalStart}.");
Console.WriteLine($"Length of the shortest path from any cell with elevation 'a': {map.BestPathLengthAnyStart}.");


