using Godot;

namespace SandboxEngine;

public static class MapController
{
    public static int Width;
    public static int Height;
    private static Cell[] _cellMap;

    public static void Init(int width, int height)
    {
        Height = height;
        Width = width;

        _cellMap = new Cell[height * width];

        for (var i = 0; i < _cellMap.Length; i++)
        {
            var position = ComputePosition(i, Width);
            _cellMap[i] = new Cell(position.X, position.Y);
        }
    }

    public static void CopyImageToMap(Image imageTexture)
    {
        for (var i = 0; i < _cellMap.Length; i++)
        {
            var coords = ComputePosition(i, Width);
            var color = imageTexture.GetPixel(coords.X, coords.Y);
            _cellMap[i].SetMaterial(Renderer.GetMaterialByColor(color));
        }
        // MapController.map[MapController.ComputePosition(coords.X, coords.y)];
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
        if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1) return false;
        return true;
    }

    public static bool IsEmpty(int x, int y)
    {
        return InBounds(x, y) && _cellMap[ComputeIndex(x, y)].Material == Materials.EMaterial.VACUUM;
    }

    public static Cell GetCellAt(int x, int y)
    {
        return _cellMap[ComputeIndex(x, y)];
    }

    // public static void SetCellAt(Cell newCell, int x, int y)
    // {
    //     Cell cell = MapController.map[MapController.ComputeIndex(x, y)];
    // }

    public static bool CompletelySurrounded(int x, int y)
    {
        // Top
        if (InBounds(x, y - 1) && !IsEmpty(x, y - 1)) return false;
        // Bottom
        if (InBounds(x, y + 1) && !IsEmpty(x, y + 1)) return false;
        // Left
        if (InBounds(x - 1, y) && !IsEmpty(x - 1, y)) return false;
        // Right
        if (InBounds(x + 1, y) && !IsEmpty(x + 1, y)) return false;
        // Top Left
        if (InBounds(x - 1, y - 1) && !IsEmpty(x - 1, y - 1)) return false;
        // Top Right
        if (InBounds(x + 1, y - 1) && !IsEmpty(x + 1, y - 1)) return false;
        // Bottom Left
        if (InBounds(x - 1, y + 1) && !IsEmpty(x - 1, y + 1)) return false;
        // Bottom Right
        if (InBounds(x + 1, y + 1) && !IsEmpty(x + 1, y + 1)) return false;

        return true;
    }

    public static void UpdateAll()
    {
        for (var i = _cellMap.Length - 1; i > 0; i--) _cellMap[i].Update(0f);
    }
}
