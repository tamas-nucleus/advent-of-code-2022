using System.Text;

namespace Day25FullOfHotAir;

internal static class SnafuConverter
{
    public static string ToSnafu(long number)
    {
        bool isNegative = number < 0;
        if (isNegative)
        {
            throw new NotImplementedException();
        }

        long toAdd = 0;
        long currentFiveValue = 1;
        while (true)
        {
            toAdd += currentFiveValue * 2;
            if (toAdd >= number)
            {
                break;
            }
            currentFiveValue *= 5;
        }
        number += toAdd;
        string baseFive = ToBaseFive(number);
        var builder = new StringBuilder();
        for (int i = 0; i < baseFive.Length; i++)
        {
            builder.Append(baseFive[i] switch
            {
                '0' => '=',
                '1' => '-',
                '2' => '0',
                '3' => '1',
                _ => '2',
            });
        }
        return builder.ToString();
    }

    public static long ToNumber(string snafu)
    {
        long number = 0;
        long powerOfFive = 1;
        for (int i = snafu.Length - 1; i >= 0; i--)
        {
            int count = snafu[i] switch
            {
                '=' => -2,
                '-' => -1,
                '0' => 0,
                '1' => 1,
                _ => 2,
            };
            number += count * powerOfFive;
            powerOfFive *= 5;
        }
        return number;

    }

    private static string ToBaseFive(long number)
    {
        if (number == 0)
        {
            return "0";
        }
        
        bool isNegative = number < 0;
        number = Math.Abs(number);
        var builder = new StringBuilder();
        while (number > 0)
        {
            long remainder = number % 5;
            number /= 5;
            builder.Append(remainder);
        }
        if (isNegative)
        {
            builder.Append('-');
        }
        int length = builder.Length;
        for (int i = 0; i < length / 2; i++)
        {
            char temp = builder[i];
            int backIndex = length - i - 1;
            builder[i] = builder[backIndex];
            builder[backIndex] = temp;
        }

        return builder.ToString();
    }
}
