using Day25FullOfHotAir;

var lineArray = File.ReadAllLines("input.txt");
long fuelSum = 0;
foreach (var snafu in lineArray)
{
    if (snafu.Length == 0)
    {
        continue;
    }

    long fuelAmount = SnafuConverter.ToNumber(snafu);
    fuelSum += fuelAmount;
}

string fuelSumSnafu = SnafuConverter.ToSnafu(fuelSum);
Console.WriteLine($"The total fuel requirement is {fuelSum}, which is '{fuelSumSnafu}' in SNAFU.");
