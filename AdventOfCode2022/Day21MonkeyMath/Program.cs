using Day21MonkeyMath;

string inputFile = "input.txt";
var calculator = Calculator.Load(inputFile);
calculator.Resolve();
Console.WriteLine($"Root is going to yell {calculator.Root.Result}.");

calculator = Calculator.Load(inputFile);
calculator.CalculateHumn();
Console.WriteLine($"I must yell {calculator.Humn.Result!.Value}.");
