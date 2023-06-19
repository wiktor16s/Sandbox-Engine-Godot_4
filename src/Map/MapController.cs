using System;
using Godot;
using SandboxEngine.Elements;

namespace SandboxEngine.Map;

public static class MapController
{
    public static int Width;
    public static int Height;
    private static Cell[] _mapBuffer;

    // private static void SwapBufferReferences()
    // {
    //     (_sourceBuffer, _destinationBuffer) = (_destinationBuffer, _sourceBuffer);
    // }

    public static void Init(int width, int height)
    {
        Height = height;
        Width = width;

        _mapBuffer = new Cell[height * width];

        for (var i = 0; i < _mapBuffer.Length; i++)
        {
            var position = ComputePosition(i, Width);
            _mapBuffer[i] = new Cell(position.X, position.Y);
        }
    }

    public static void CopyImageToMap(Image imageTexture)
    {
        for (var i = 0; i < _mapBuffer.Length; i++)
        {
            var coords = ComputePosition(i, Width);
            var color = imageTexture.GetPixel(coords.X, coords.Y);
            _mapBuffer[i].SetMaterial(Renderer.GetMaterialByColor(color));
        }
    }

    public static int ComputeIndex(int x, int y)
    {
        return y * Width + x;
    }

    public static Vector2I ComputeReversePosition(int index, int width)
    {
        var x = Width - index % width - 1;
        var y = Height - index / width - 1;
        return new Vector2I(x, y);
    }

    public static Vector2I ComputePosition(int index, int width)
    {
        return new Vector2I(
            index % width,
            index / width
        );
    }

    public static bool InBounds(int x, int y)
    {
        return x >= 0 && x <= Width - 1 && y >= 0 && y <= Height - 1;
    }

    public static bool InBounds(Vector2I position)
    {
        return InBounds(position.X, position.Y);
    }

    public static Vector2I NormalizePosition(Vector2I position)
    {
        // if (position.X > Width)
        //     position.X = Width + 1;
        // if (position.X < 0)
        //     position.X = 0;
        if (position.Y > Height - 1)
            position.Y = Height - 1;
        if (position.Y < 0)
            position.Y = 0;
        return position;
    }

    public static EMaterial GetCellMaterialFromMapBuffer(int x, int y)
    {
        if (!InBounds(x, y)) throw new Exception("Trying to reach out of bounds " + x + ", " + y);
        return _mapBuffer[ComputeIndex(x, y)].Material;
    }

    public static EMaterial GetCellMaterialFromMapBuffer(Vector2I position)
    {
        return GetCellMaterialFromMapBuffer(position.X, position.Y);
    }

    public static Cell GetCellFromMapBuffer(int x, int y)
    {
        if (!InBounds(x, y)) throw new Exception("Trying to reach out of bounds " + x + ", " + y);
        return _mapBuffer[ComputeIndex(x, y)];
    }

    public static Cell GetCellFromMapBuffer(Vector2I position)
    {
        return GetCellFromMapBuffer(position.X, position.Y);
    }

    public static void UpdateAll()
    {
        Globals.tickOscillator = !Globals.tickOscillator;

        for (var i = 0; i < Height; i++)
            if (i % 2 == 0)
                for (var j = 0; j < Width; j++)
                    _mapBuffer[i * Width + j].Update(0f);
            else
                for (var j = Width - 1; j > 0; j--)
                    _mapBuffer[i * Width + j].Update(0f);
    }
}