using System;
using Godot;

namespace SandboxEngine;

public static class Utils
{
    private static readonly Random        Generator = new();
    public static readonly  FastNoiseLite Noise     = new();

    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(max) > 0) return max;

        if (value.CompareTo(min) < 0) return min;

        return value;
    }

    public static bool GetRandomBool()
    {
        return Generator.Next() % 2 == 0;
    }

    public static int GetRandomInt(int min, int max)
    {
        return Generator.Next(min, max);
    }

    public static double GetRandomDouble(double min, double max)
    {
        return Generator.NextDouble() * (max - min) + min;
    }

    public static float GetRandomFloat(float min, float max)
    {
        if (min >= max) throw new ArgumentException("Invalid range. Max value must be greater than min value.");

        return (float)(Generator.NextDouble() * (max - min) + min);
    }

    public static Vector2I[] GetShortestPathBetweenTwoCells(Vector2I pos1, Vector2I pos2, Renderer renderer)
    {
        var pos2ParentRenderer = renderer;

        if (!renderer.InBounds(pos1)) pos1 = renderer.NormalizePosition(pos1);
        if (!renderer.InBounds(pos2))
        {
            pos2ParentRenderer = RenderManager.GetRendererByRelativePosition(pos2, renderer);
            if (pos2ParentRenderer is null)
            {
                pos2ParentRenderer = renderer;
                pos2               = renderer.NormalizePosition(pos2);
            }
        }

        if (pos1 == pos2) return new[] { pos1 };

        var matrixX1 = pos1.X;
        var matrixY1 = pos1.Y;
        var matrixX2 = pos2.X;
        var matrixY2 = pos2.Y;

        var xDiff         = matrixX1 - matrixX2;
        var yDiff         = matrixY1 - matrixY2;
        var xDiffIsLarger = Math.Abs(xDiff) > Math.Abs(yDiff);

        var xModifier = xDiff < 0 ? 1 : -1;
        var yModifier = yDiff < 0 ? 1 : -1;

        var longerSideLength  = Math.Max(Math.Abs(xDiff), Math.Abs(yDiff));
        var shorterSideLength = Math.Min(Math.Abs(xDiff), Math.Abs(yDiff));
        var slope             = shorterSideLength == 0 || longerSideLength == 0 ? 0 : (float)shorterSideLength / longerSideLength;

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

            var currentPossition = new Vector2I(
                matrixX1 + xIncrease * xModifier,
                matrixY1 + yIncrease * yModifier
            );

            if (
                renderer.InBounds(currentPossition) ||
                pos2ParentRenderer.InBounds(RenderManager.GetOffsetOfRelativePosition(currentPossition)))
            {
                path[i - 1] = currentPossition;
            }
        }

        return path;
    }

    public static float Normalize(float x, float min, float max)
    {
        if (x < min) return min;

        if (x > max) return max;

        var range           = max - min;
        var normalizedValue = (x - min) / range;
        return normalizedValue;
    }

    public static float MapValue(float a, float a0, float a1, float b0, float b1)
    {
        return b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
    }

    public static Color ModifyColor(Color originalColor, byte range, bool red = true, bool green = true, bool blue = true)
    {
        var modifiedColor = new Color(
            originalColor.R,
            originalColor.G,
            originalColor.B,
            0.2f
        );
        var redColor    = (byte)originalColor.R;
        var greenColor  = (byte)originalColor.G;
        var blueColor   = (byte)originalColor.B;
        var changeRed   = 0f;
        var changeGreen = 0f;
        var changeBlue  = 0f;

        if (red) changeRed = (float)(Generator.NextDouble() * 2 - 1) * range;

        if (green) changeGreen = (float)(Generator.NextDouble() * 2 - 1) * range;

        if (blue) changeBlue = (float)(Generator.NextDouble() * 2 - 1) * range;

        var newRedColor   = redColor   + changeRed;
        var newGreenColor = greenColor + changeGreen;
        var newBlueColor  = blueColor  + changeBlue;

        newRedColor   = Math.Min(255, newRedColor);
        newRedColor   = Math.Max(0, newRedColor);
        newGreenColor = Math.Min(255, newGreenColor);
        newGreenColor = Math.Max(0, newGreenColor);
        newBlueColor  = Math.Min(255, newBlueColor);
        newBlueColor  = Math.Max(0, newBlueColor);
        modifiedColor = new Color(
            Normalize(newRedColor,   0, 255),
            Normalize(newGreenColor, 0, 255),
            Normalize(newBlueColor,  0, 255)
        );


        return modifiedColor;
    }

    public static Color Darken(Color color, float maxChange)
    {
        var colorChange = GetRandomFloat(0f, maxChange);
        var newColor = new Color(
            Normalize(color.R - colorChange, 0, 255),
            Normalize(color.G - colorChange, 0, 255),
            Normalize(color.B - colorChange, 0, 255)
        );
        return newColor;
    }
}