using System;
using Godot;
using SandboxEngine.Map;

namespace SandboxEngine;

public static class Utils
{
    private static readonly Random Generator = new();

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

    public static Vector2I[] GetShortestPathBetweenTwoCells(Vector2I pos1, Vector2I pos2)
    {
        // If the two points are the same no need to iterate. Just run the provided function
        if (pos1 == pos2)
        {
            return new[] { pos1 };
        }

        var matrixX1 = pos1.X;
        var matrixY1 = pos1.Y;
        var matrixX2 = pos2.X;
        var matrixY2 = pos2.Y;

        var xDiff = matrixX1 - matrixX2;
        var yDiff = matrixY1 - matrixY2;
        var xDiffIsLarger = Math.Abs(xDiff) > Math.Abs(yDiff);

        var xModifier = xDiff < 0 ? 1 : -1;
        var yModifier = yDiff < 0 ? 1 : -1;

        var longerSideLength = Math.Max(Math.Abs(xDiff), Math.Abs(yDiff));
        var shorterSideLength = Math.Min(Math.Abs(xDiff), Math.Abs(yDiff));
        var slope = shorterSideLength == 0 || longerSideLength == 0 ? 0 : (float)shorterSideLength / longerSideLength;

        var path = new Vector2I[longerSideLength];

        int shorterSideIncrease;
        for (var i = 1; i <= longerSideLength; i++)
        {
            shorterSideIncrease = (int)Math.Round(i * slope);
            int yIncrease, xIncrease;
            if (xDiffIsLarger)
            {
                xIncrease = i;
                yIncrease = shorterSideIncrease;
            }
            else
            {
                yIncrease = i;
                xIncrease = shorterSideIncrease;
            }

            var currentY = matrixY1 + yIncrease * yModifier;
            var currentX = matrixX1 + xIncrease * xModifier;
            if (MapController.InBounds(currentX, currentY))
            {
                path[i - 1] = new Vector2I(currentX, currentY);
            }
        }

        return path;
    }

    public static double Normalize(double value, double minValue, double maxValue)
    {
        if (minValue == maxValue) throw new ArgumentException("MinValue and MaxValue must be different.");
        var normalizedValue = (value - minValue) / (maxValue - minValue);
        normalizedValue = Math.Max(0, Math.Min(1, normalizedValue));
        return normalizedValue;
    }
}