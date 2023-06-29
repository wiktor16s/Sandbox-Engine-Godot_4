using System;

public class Program
{
    public static void Main()
    {
        Console.WriteLine(IntToRoman(1994));
    }

    public static string IntToRoman(int num)
    {
        var rest   = 0;
        var result = "";

        while (num > 0)
        {
            var leading = Utils.GetLeadingPartOfNumber(num);
            rest = Utils.GetRemainingPartOfNumber(num);
            Console.WriteLine("default: " + num);
            Console.WriteLine("lean "     + leading);
            Console.WriteLine("rest "     + rest);

            if (leading == 0)
            {
                Console.WriteLine("lean is 0");
                leading = rest;
                rest    = 0;
            }

            var highest   = Utils.GetHighestValFromRange(leading);
            var multipler = leading / highest;
            Console.WriteLine("high " + highest);
            Console.WriteLine("mult " + multipler);
            for (var i = 0; i < multipler; i++)
            {
                result += Utils.GetRomanByInt(highest);
            }

            num  = leading - highest * Math.Max(multipler, 1) + rest;
            rest = 0;

            Console.WriteLine("num " + num);
            Console.WriteLine(" ");
        }

        return result;
    }
}

public static class Utils
{
    public enum RomanToIntMap
    {
        I  = 1,
        IV = 4,
        V  = 5,
        IX = 9,
        X  = 10,
        XL = 40,
        L  = 50,
        XC = 90,
        C  = 100,
        CD = 400,
        D  = 500,
        CM = 900,
        M  = 1000
    }

    public static int GetHighestValFromRange(int num)
    {
        if (num >= 1 && num <= 3)
        {
            return (int)RomanToIntMap.I; // 1;3 = I
        }

        if (num >= 4 && num <= 8)
        {
            return (int)RomanToIntMap.V; // 4;8 = V 
        }

        if (num >= 9 && num <= 39)
        {
            return (int)RomanToIntMap.X; // 9;39 = X
        }

        if (num >= 40 && num <= 89)
        {
            return (int)RomanToIntMap.L; // 40;89 = L
        }

        if (num >= 90 && num <= 399)
        {
            return (int)RomanToIntMap.C; // 90;899 = C
        }

        if (num >= 400 && num <= 899)
        {
            return (int)RomanToIntMap.D; // 400;899 = D 
        }

        if (num >= 900 && num <= 3999)
        {
            return (int)RomanToIntMap.M; // 900;399 = M
        }

        return 0;
        // throw new ArgumentOutOfRangeException("Parameter index is out of range.", num);
    }

    public static int GetLeadingPartOfNumber(int num)
    {
        var leadingPart = 0;
        if (num < 10)
        {
            return 0;
        }

        var power = (int)Math.Pow(10, (int)Math.Log10(num));
        leadingPart = num / power * power;
        return leadingPart;
    }

    public static int GetRemainingPartOfNumber(int num)
    {
        var remainingPart = 0;
        if (num < 10)
        {
            return num;
        }

        var power = (int)Math.Pow(10, (int)Math.Log10(num));
        remainingPart = num % power;
        return remainingPart;
    }

    public static string GetRomanByInt(int val)
    {
        return Enum.GetName(typeof(RomanToIntMap), val);
    }
}