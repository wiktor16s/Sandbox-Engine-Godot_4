using Godot;
using SandboxEngine.Materials.Solid.Movable;

namespace SandboxEngine;

public class Cell 
{
    public bool HasBeenUpdatedThisFrame;

    public Cell(int x, int y)
    {
        Position = new Vector2I(x, y);
        Material = Materials.None.Material;
        HasBeenUpdatedThisFrame = true;
    }

    public uint Material { get; private set; }
    public float Lifetime { get; private set; } // in ticks
    public Vector2I Position { get; }
    public Vector2 Velocity { get; set; }


    public void SetMaterialByColor(Color color)
    {
        var colorI = color.ToRgba32();
        switch (colorI)
        {
            case Sand.Material:
                Material = Sand.Material;
                break;
            case Materials.None.Material:
                Material = Materials.None.Material;
                break;
            case Materials.Stone.Material:
                Material = Materials.Stone.Material;
                break;
            case Materials.Water.Material:
                Material = Materials.Water.Material;
                break;
            default:
                Material = Materials.Unknown.Material;
                break;
        }
    }

    public void SetMaterial(uint material) // todo change type to interface
    {
        Material = material;
    }

    public Color GetMaterialColor()
    {
        switch (Material)
        {
            case Materials.None.Material:
                return new Color(Materials.None.Color);
            case Materials.Stone.Material:
                return new Color(Materials.Stone.Color);
            case Sand.Material:
                return new Color(Sand.Color);
            case Materials.Water.Material:
                return new Color(Materials.Water.Color);
            default:
                return new Color(Materials.Unknown.Color);
        }
    }

    public void Update(float tickDeltaTime)
    {
        switch (Material)
        {
            case Sand.Material:
                Sand.Update();
                break;
        }
    }

    public void Move(int x, int y)
    {
        var newLocationCell = MapController.GetCellAt(x, y);
        newLocationCell.Material = Material;
        newLocationCell.Lifetime = Lifetime;
        newLocationCell.Velocity = Velocity;
        newLocationCell.HasBeenUpdatedThisFrame = true;

        //When cell moved, set defaults at this Cell`s position
        SetDefaults();
    }

    private void SetDefaults()
    {
        Material = Materials.None.Material;
        Lifetime = 0;
        Velocity = new Vector2I(0, 0);
        HasBeenUpdatedThisFrame = true;
    }
}