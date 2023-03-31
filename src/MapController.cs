using System;
using Godot;
using SandboxEngine.Materials;
using static SandboxEngine.Globals;

namespace SandboxEngine;

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
    
    // public static void SetCellAt(Cell newCell, int x, int y)
    // {
    //     Cell cell = MapController.map[MapController.ComputeIndex(x, y)];
    // }

    // public static bool CompletelySurrounded(int x, int y)
    // {
    //     // Top
    //     if (InBounds(x, y - 1) && !IsEmpty(x, y - 1)) return false;
    //     // Bottom
    //     if (InBounds(x, y + 1) && !IsEmpty(x, y + 1)) return false;
    //     // Left
    //     if (InBounds(x - 1, y) && !IsEmpty(x - 1, y)) return false;
    //     // Right
    //     if (InBounds(x + 1, y) && !IsEmpty(x + 1, y)) return false;
    //     // Top Left
    //     if (InBounds(x - 1, y - 1) && !IsEmpty(x - 1, y - 1)) return false;
    //     // Top Right
    //     if (InBounds(x + 1, y - 1) && !IsEmpty(x + 1, y - 1)) return false;
    //     // Bottom Left
    //     if (InBounds(x - 1, y + 1) && !IsEmpty(x - 1, y + 1)) return false;
    //     // Bottom Right
    //     if (InBounds(x + 1, y + 1) && !IsEmpty(x + 1, y + 1)) return false;
    //
    //     return true;
    // }

    public static void UpdateAll()
    {
        Globals.tickOscillator = !Globals.tickOscillator;
        //GD.Print(UpdateTickCounter);
        //for (var i = _mapBuffer.Length - 1; i > 0; i--) _mapBuffer[i].Update(0f);
        for (var i = 0; i < _mapBuffer.Length; i++) _mapBuffer[i].Update(0f);
        //for (var i = _mapBuffer.Length - 1; i > 0; i--) _mapBuffer[i].HasBeenUpdatedThisFrame = false;
    }
}