using Day23UnstableDiffusion;

var map = Map.Load("input.txt");
map.Spread(10);
Console.WriteLine($"After spreading out {map.SpreadCount} times, " +
    $"the number of empty tiles is {map.GetEmptyTileCount()}.");

map.Spread();
Console.WriteLine($"The first round where no elf moved was {map.SpreadCount + 1}.");
