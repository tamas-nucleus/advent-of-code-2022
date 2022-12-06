using Day6TuningTrouble;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLineArray = File.ReadAllLines(inputFile);
var packetStartDecoder = new MessageDecoder(4);
var messageStartDecoder = new MessageDecoder(14);
foreach (var message in inputLineArray)
{
    if (message.Length == 0)
    {
        continue;
    }

    packetStartDecoder.ProcessNewMessage(message);
    messageStartDecoder.ProcessNewMessage(message);
    Console.WriteLine($"first packet marker after character {packetStartDecoder.MarkerLastCharacterIndex}");
    Console.WriteLine($"first message marker after character {messageStartDecoder.MarkerLastCharacterIndex}");
}