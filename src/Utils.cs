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

    public static double GetRandomInt(int min, int max)
    {
        return Generator.Next(min, max);
    }

    public static double GetRandomDouble(double min, double max)
    {
        return Generator.NextDouble() * (max - min) + min;
    }

    public static float GetRandomFloat(float min, float max)
    {
        if (min >= max)
        {
            throw new ArgumentException("Invalid range. Max value must be greater than min value.");
        }
        
        return (float)(Generator.NextDouble() * (max - min) + min);
    }
    
    public static Vector2I[] GetShortestPathBetweenTwoCells(Vector2I pos1, Vector2I pos2)
    {
        if (!MapController.InBounds(pos1)) pos1 = MapController.NormalizePosition(pos1);
        if (!MapController.InBounds(pos2)) pos2 = MapController.NormalizePosition(pos2);

        // If the two points are the same no need to iterate. Just run the provided function
        if (pos1 == pos2) return new[] { pos1 };

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
            if (MapController.InBounds(currentX, currentY)) path[i - 1] = new Vector2I(currentX, currentY);
        }

        return path;
    }

    public static float Normalize(float x, float a, float b)
    {
        if (x < a)
            return a;
        if (x > b)
            return b;
        var range = b - a;
        var normalizedValue = (x - a) / range;
        return normalizedValue;
    }

    public static Color ModifyColor(Color originalColor, byte range, bool red = true, bool green = true, bool blue = true)
    {
        var modifiedColor = new Color(
            originalColor.R,
            originalColor.G,
            originalColor.B,
            0.2f
        );
        var redColor = (byte)originalColor.R;
        var greenColor = (byte)originalColor.G;
        var blueColor = (byte)originalColor.B;
        var changeRed = 0f;
        var changeGreen = 0f;
        var changeBlue = 0f;
        
        if (red) changeRed = (float)(Generator.NextDouble() * 2 - 1) * range;
        if (green) changeGreen = (float)(Generator.NextDouble() * 2 - 1) * range;
        if (blue) changeBlue = (float)(Generator.NextDouble() * 2 - 1) * range;
        
        var newRedColor = redColor + changeRed;
        var newGreenColor = greenColor + changeGreen;
        var newBlueColor = blueColor + changeBlue;
        
        newRedColor = Math.Min(255, newRedColor);
        newRedColor = Math.Max(0, newRedColor);
        newGreenColor = Math.Min(255, newGreenColor);
        newGreenColor = Math.Max(0, newGreenColor);
        newBlueColor = Math.Min(255, newBlueColor);
        newBlueColor = Math.Max(0, newBlueColor);
        modifiedColor = new Color(
            Normalize(newRedColor, 0, 255),
            Normalize(newGreenColor, 0, 255),
            Normalize(newBlueColor, 0, 255)
        );

        //GD.Print($"R: {modifiedColor.R} G: {modifiedColor.G} B: {modifiedColor.B}");

        return modifiedColor;
    }

    public static Color Darken(Color color, float maxChange)
    {
        //if (maxChange < 0 || maxChange > 1) throw new ArgumentException("Percentage should be between 0 and 100.");
    
        var colorChange = GetRandomFloat(0f, maxChange);
        GD.Print(colorChange);

        var newColor = new Color(
            Normalize(color.R - colorChange, 0, 255),
            Normalize(color.G - colorChange, 0, 255),
            Normalize(color.B - colorChange, 0, 255)
        );
        return newColor;
    }
}