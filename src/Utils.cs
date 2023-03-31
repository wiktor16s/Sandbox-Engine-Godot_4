using System;

namespace SandboxEngine;

public static class Utils
{
    public static Random Generator = new Random();
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(max) > 0)
            return max;
        if (value.CompareTo(min) < 0)
            return min;
        return value;
    }

    public static bool GetRandomBool()
    {
        return Generator.Next() % 2 == 0;
    }
    
}