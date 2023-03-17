using Godot;

namespace SandboxEngine;

public static class MapController
{
    public const float Gravity = 10f;

    public static int Width;
    public static int Height;
    private static Cell[] _map;

    public static void Init(int width, int height)
    {
        Height = height;
        Width = width;

        _map = new Cell[height * width];

        for (var i = 0; i < _map.Length; i++)
        {
            var position = ComputePosition(i, Width);
            _map[i] = new Cell(position.X, position.Y);
        }
    }

    public static void CopyImageToMap(Image imageTexture)
    {
        //GD.Print(MapController.map.Length);

        for (var i = 0; i < _map.Length; i++)
        {
            var coords = ComputePosition(i, Width);
            //GD.Print("coords:", coords, " index: ", i);
            var color = imageTexture.GetPixel(coords.X, coords.Y);
            //GD.Print("color:", color);
            _map[i].SetMaterialByColor(color);
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
        return InBounds(x, y) && _map[ComputeIndex(x, y)].Material == EMaterial.NONE;
    }

    public static Cell GetCellAt(int x, int y)
    {
        return _map[ComputeIndex(x, y)];
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
        for (var i = _map.Length - 1; i > 0; i--) _map[i].Update(0f);
    }
}

/*

for map 4x4

15 14 13 12
11 10  9  8
 7  6  5  4
 3  2  1  0

is exual to

[ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 ]

we need to iterate from bottom to top

*/