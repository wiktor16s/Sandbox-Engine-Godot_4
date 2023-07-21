using System;
using Godot;
using SandboxEngine.Map;

namespace SandboxEngine.Utils;

public static class Tools
{
    private static readonly Random        Generator = new();
    public static readonly  FastNoiseLite Noise     = new();

    public static Cell[][] SplitCellArrayIntoSquareChunks_v2(Cell[] mapBuffer)
    {
        int i, j;
        var n      = mapBuffer.Length;
        var k      = (int)Math.Round(Math.Sqrt(n) / 2);
        var result = new Cell[4][];
        for (i = 0; i < 4; i++)
            result[i] = new Cell[n / 4];

        int tOff = 0, sOff = 0;

        i = 0;
        for (j = 0; j < k; j++)
        {
            tOff += k;
            for (; i < tOff; i++)
                result[0][i] = mapBuffer[sOff + i];
            sOff += k;
        }

        sOff = k;
        tOff = 0;
        i    = 0;
        for (j = 0; j < k; j++)
        {
            tOff += k;
            while (i < tOff)
                result[1][i] = mapBuffer[sOff + i++];
            sOff += k;
        }

        sOff = n / 2;
        tOff = 0;
        i    = 0;
        for (j = 0; j < k; j++)
        {
            tOff += k;
            while (i < tOff)
                result[2][i] = mapBuffer[sOff + i++];
            sOff += k;
        }

        sOff = n / 2 + k;
        tOff = 0;
        i    = 0;
        for (j = 0; j < k; j++)
        {
            tOff += k;
            while (i < tOff)
                result[3][i] = mapBuffer[sOff + i++];
            sOff += k;
        }

        return result;
    }


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

    public static Vector2I[]
        GetShortestPathBetweenTwoCells(Vector2I pos1, Vector2I pos2, Renderer renderer)
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

    public static Cell[][] SplitCellArrayIntoSquareChunks(Cell[] mapBuffer)
    {
        var halfLength = mapBuffer.Length / 2;

        var size     = Mathf.RoundToInt(Mathf.Sqrt(mapBuffer.Length));
        var halfSize = size / 2;

        var splitVals = new Cell[4][];
        for (var i = 0; i < splitVals.Length; i++)
            splitVals[i] = new Cell[mapBuffer.Length / 4];

        for (var groupIndex = 0; groupIndex < 4; groupIndex++)
        {
            var initialGroupIndex = groupIndex / 2 * halfLength + groupIndex % 2 * halfSize;
            for (var x = 0; x < halfSize; x++)
                for (var y = 0; y < halfSize; y++)
                    splitVals[groupIndex][x * halfSize + y] = mapBuffer[initialGroupIndex + x * size + y];
        }

        return splitVals;
    }

    public static Vector2I ComputePosition(int index, int width, int height)
    {
        return new Vector2I(
            index % width,
            index / height
        );
    }

    public static int ComputeIndex(Vector2I position, int size)
    {
        return position.Y * size + position.X;
    }
}