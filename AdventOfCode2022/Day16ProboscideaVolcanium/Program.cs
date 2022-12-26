using Day16ProboscideaVolcanium;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var map = new Map() { PrintDebugMessages = true };
map.LoadInput(inputFile);
map.DropEmptyTunnels();
map.MeasureDistances();
map.CalculateMaxFlowRate(26, 2);
