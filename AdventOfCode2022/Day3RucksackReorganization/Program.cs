using System.Numerics;

string inputFile = (args.Length == 0) ? "input.txt" : args[0];
var inputLineArray = File.ReadAllLines(inputFile);
int wrongItemPrioritySum = 0;
int badgePrioritySum = 0;
ulong groupBits = ulong.MaxValue;
for (int i = 0; i < inputLineArray.Length; i++)
{
    string rucksack = inputLineArray[i];
    if (rucksack.Length == 0)
    {
        continue;
    }

    // Finding the wrong item.
    int compartmentSize = rucksack.Length / 2;
    var firstCompartment = rucksack.AsSpan(0, compartmentSize);
    var secondCompartment = rucksack.AsSpan(compartmentSize);
    foreach (var item in firstCompartment)
    {
        if (!secondCompartment.Contains(item))
        {
            continue;
        }

        wrongItemPrioritySum += GetPriority(item);
        break;
    }

    // Finding the badge.
    ulong rucksackBits = 0;
    foreach (var item in rucksack)
    {
        int priority = GetPriority(item);
        rucksackBits |= 1ul << priority;
    }
    groupBits &= rucksackBits;

    if (i % 3 == 2)
    {
        badgePrioritySum += GetPriorityOfGroupBadge(groupBits);
        groupBits = ulong.MaxValue;
    }
}

if (groupBits != ulong.MaxValue)
{
    badgePrioritySum += GetPriorityOfGroupBadge(groupBits);
}

Console.WriteLine($"Wrong item priority sum: {wrongItemPrioritySum}");
Console.WriteLine($"Badge priority sum: {badgePrioritySum}");

int GetPriority(char item)
{
    byte itemAsByte = (byte)item;
    return (itemAsByte >= (byte)'a')
        ? 1 + itemAsByte - (byte)'a'
        : 27 + itemAsByte - (byte)'A';
}

int GetPriorityOfGroupBadge(ulong groupBits)
{
    return BitOperations.TrailingZeroCount(groupBits);
}