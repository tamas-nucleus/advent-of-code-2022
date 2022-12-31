using Day24BlizzardBasin;

var map = Map.Load("input.txt");
map.FindFastestPath();
Console.WriteLine($"It takes at least {map.Time} minutes to cross the valley.");
map.FindFastestPath(reverse: true);
map.FindFastestPath();
Console.WriteLine($"After returning with the snacks, the elapsed time is {map.Time} minutes.");
