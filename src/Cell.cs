using Godot;

namespace SandboxEngine;

public class Cell
{
    public bool HasBeenUpdatedThisFrame;

    public Cell(int x, int y)
    {
        Position = new Vector2I(x, y);
        Material = EMaterial.NONE;
        HasBeenUpdatedThisFrame = true;
    }

    public EMaterial Material { get; private set; }
    public float Lifetime { get; private set; } // in ticks
    public Vector2I Position { get; }
    public Vector2 Velocity { get; private set; }

    public void SetMaterialByColor(Color color)
    {
        var colorI = color.ToRgba32();
        Material = colorI switch
        {
            (uint)EMaterial.SAND => EMaterial.SAND,
            (uint)EMaterial.NONE => EMaterial.NONE,
            (uint)EMaterial.WALL => EMaterial.WALL,
            (uint)EMaterial.WATER => EMaterial.WATER,
            _ => EMaterial.UNKNOWN
        };
    }

    public void SetMaterial(EMaterial material)
    {
        Material = material;
    }

    public Color GetMaterialColor()
    {
        return Material switch
        {
            EMaterial.NONE => new Color((uint)EMaterial.NONE),
            EMaterial.WALL => new Color((uint)EMaterial.WALL),
            EMaterial.SAND => new Color((uint)EMaterial.SAND),
            EMaterial.WATER => new Color((uint)EMaterial.WATER),
            _ => new Color((uint)EMaterial.UNKNOWN)
        };
    }

    public void Update(float tickDeltaTime)
    {
        switch (Material)
        {
            case EMaterial.SAND:
                UpdateAsSand(tickDeltaTime);
                break;
        }
    }

    private void Move(int x, int y)
    {
        var newLocationCell = MapController.GetCellAt(x, y);
        newLocationCell.Material = Material;
        newLocationCell.Lifetime = Lifetime;
        newLocationCell.Velocity = Velocity;
        newLocationCell.HasBeenUpdatedThisFrame = true;
        Reset();
    }

    private void Reset()
    {
        Material = EMaterial.NONE;
        Lifetime = 0;
        Velocity = new Vector2I(0, 0);
        HasBeenUpdatedThisFrame = true;
    }

    private void UpdateAsSand(float tickDeltaTime)
    {
        var down = 0;
        var left = false;
        var right = false;
        const int gravitation = 4;

        var gravitationVector = new Vector2(0, 1);

        // if (position.Y + 1 < MapController.height)
        // {
        for (var i = 1; i < gravitation + 1; i++)
            if (MapController.IsEmpty(Position.X, Position.Y + i))
                down = i;
            else
                break;
        if (Position.X - 1 >= 0) left = MapController.IsEmpty(Position.X - 1, Position.Y + 1);
        if (Position.X + 1 < MapController.Height) right = MapController.IsEmpty(Position.X + 1, Position.Y + 1);
        //}

        if (left && right)
        {
            var rand = GD.Randi() % 2 == 1;
            left = rand;
            right = !rand;
        }

        if (down > 0)
        {
            Velocity = Velocity + new Vector2I(0, 1);
            Move(Position.X, Position.Y + down);
            Renderer.DrawCell(new Vector2I(Position.X, Position.Y + down), EMaterial.SAND);
        }
        else if (left)
        {
            Move(Position.X - 1, Position.Y + 1);
            Renderer.DrawCell(new Vector2I(Position.X - 1, Position.Y + 1), EMaterial.SAND);
        }
        else if (right)
        {
            Move(Position.X + 1, Position.Y + 1);
            Renderer.DrawCell(new Vector2I(Position.X + 1, Position.Y + 1), EMaterial.SAND);
        }

        if (down > 0 || left || right) Renderer.DrawCell(new Vector2I(Position.X, Position.Y), EMaterial.NONE);
    }
}