using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Materials;
using SandboxEngine.Materials.Solid.Movable;

namespace SandboxEngine;

public class Cell
{
    public bool HasBeenUpdatedThisFrame;
    public EMaterial Material;
    public Vector2I Position;
    public int Temperature;
    public Vector2I Velocity;
    public uint Lifetime;

    public Cell(int x, int y)
    {
        HasBeenUpdatedThisFrame = true;
        Material = EMaterial.VACUUM;
        Position = new Vector2I(x, y);
        Temperature = 0;
        Velocity = new Vector2I(0, 0);
        Lifetime = 0;
    }
    
    // public void SetMaterialByColor(Color color)
    // {
    //     var colorI = color.ToRgba32();
    //     switch (colorI)
    //     {
    //         case Materials.Solid.Movable.Sand.
    //             Material = Sand.Material;
    //             break;
    //         case None.Material:
    //             Material = None.Material;
    //             break;
    //         case Stone.Material:
    //             Material = Stone.Material;
    //             break;
    //         case Water.Material:
    //             Material = Water.Material;
    //             break;
    //         default:
    //             Material = Unknown.Material;
    //             break;
    //     }
    // }

    public void SetMaterial(Materials.EMaterial material) // todo change type to interface
    {
        Material = material;
    }

    public void Update(float tickDeltaTime)
    {
        switch (Material)
        {
            case Materials.EMaterial.SAND:
                CellPool.Sand.Update(this);
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
        Material = CellPool.Vacuum.Material;
        Lifetime = CellPool.Vacuum.DefaultValues.Lifetime;
        Velocity = CellPool.Vacuum.DefaultValues.Velocity;
        HasBeenUpdatedThisFrame = true;
    }
}