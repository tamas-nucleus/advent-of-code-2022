using Day22MonkeyMap;

string inputFile = "input.txt";
var map = Map.Load(inputFile);
map.FollowInstructions();
Console.WriteLine($"Arrived at {map.FinalPosition}, facing {map.FinalDirection}.");
Console.WriteLine($"The resulting password is {map.Password}.");

var cubeMap = CubeMap.Load(inputFile);
int cubePassword = cubeMap.FollowInstructions();
Console.WriteLine($"The real password, however, is {cubePassword}.");
