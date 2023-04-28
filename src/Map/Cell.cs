using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Elements;

namespace SandboxEngine.Map;

public class Cell
{
    public readonly Vector2I ConstPosition;
    public bool IsFalling;

    public bool LastUpdatedInTick;
    public uint Lifetime;
    public EMaterial Material;
    public Vector2 PositionOffset;
    public int Temperature;
    public Vector2 Velocity;

    public Cell(int x, int y)
    {
        //HasBeenUpdatedThisFrame = true;
        Material = EMaterial.VACUUM;
        PositionOffset = new Vector2(0, 0);
        ConstPosition = new Vector2I(x, y);
        Temperature = 0;
        Lifetime = 0;
        LastUpdatedInTick = Globals.tickOscillator;
        IsFalling = true;
    }

    public void SetMaterial(EMaterial material) // todo change type to interface
    {
        Material = material;
    }

    public int CheckFreeCellsDown(int amount, Vector2I direction)
    {
        var freeCells = 0;
        for (var i = 1; i < amount + 1; i++)
        {
            if (
                MapController.InBounds(ConstPosition.X, ConstPosition.Y + i) &&
                CheckCellIsDenserThanTargetCell(new Vector2I(ConstPosition.X, ConstPosition.Y + i)) &&
                CheckTargetCellIsMovable(new Vector2I(ConstPosition.X, ConstPosition.Y + i))
            )
            {
                freeCells = i;
            }
            else
            {
                break;
            }
        }

        return freeCells;
    }
    
    
    
    public int CheckFreeCells(int amount, Vector2I direction)
    {
        var freeCells = 0;
        for (var i = 1; i < amount + 1; i++)
        {
            if (!MapController.InBounds(ConstPosition.X + direction.X * i, ConstPosition.Y + direction.Y * i)) return freeCells;

            var nextElement = MaterialPool.GetByMaterial(
                MapController.GetCellFromMapBuffer(ConstPosition.X + direction.X * i, ConstPosition.Y + direction.Y * i).Material
            );

            if (
                nextElement.Defaults.Density < MaterialPool.GetByMaterial(Material).Defaults.Density &&
                nextElement.Substance is ESubstance.AIR or ESubstance.FLUID or ESubstance.VACUUM
            )
            {
                freeCells = i;
            }
            else
            {
                break;
            }
        }
        return freeCells;
    }

    public bool CheckCellIsDenserThanTargetCell(Vector2I targetPosition)
    {
        return MaterialPool.GetByMaterial(
            MapController.GetCellFromMapBuffer(targetPosition.X, targetPosition.Y).Material
        ).Defaults.Density < MaterialPool.GetByMaterial(Material).Defaults.Density;
    }

    public bool CheckTargetCellIsMovable(Vector2I targetPosition)
    {
        return MaterialPool.GetByMaterial(
            MapController.GetCellFromMapBuffer(targetPosition.X, targetPosition.Y).Material
        ).Substance is ESubstance.AIR or ESubstance.FLUID or ESubstance.VACUUM;
    }

    public bool checkFreeCellsOnLeftDown()
    {
        return MapController.InBounds(ConstPosition.X - 1, ConstPosition.Y + 1) &&
               MaterialPool.GetByMaterial(
                   MapController.GetCellFromMapBuffer(ConstPosition.X - 1,
                       ConstPosition.Y + 1).Material
               ).Defaults.Density < MaterialPool.GetByMaterial(Material).Defaults.Density;
    }

    public bool checkFreeCellsOnLeft()
    {
        return MapController.InBounds(ConstPosition.X - 1, ConstPosition.Y) &&
               MaterialPool.GetByMaterial(
                   MapController.GetCellFromMapBuffer(ConstPosition.X - 1,
                       ConstPosition.Y).Material
               ).Defaults.Density < MaterialPool.GetByMaterial(Material).Defaults.Density;
    }

    public bool checkFreeCellsOnRightDown()
    {
        return MapController.InBounds(ConstPosition.X + 1, ConstPosition.Y + 1) &&
               MaterialPool.GetByMaterial(
                   MapController.GetCellFromMapBuffer(ConstPosition.X + 1,
                       ConstPosition.Y + 1).Material
               ).Defaults.Density < MaterialPool.GetByMaterial(Material).Defaults.Density;
    }

    public bool checkFreeCellsOnRigh()
    {
        return MapController.InBounds(ConstPosition.X + 1, ConstPosition.Y) &&
               MaterialPool.GetByMaterial(
                   MapController.GetCellFromMapBuffer(ConstPosition.X + 1,
                       ConstPosition.Y).Material
               ).Defaults.Density < MaterialPool.GetByMaterial(Material).Defaults.Density;
    }


    public void Update(float tickDeltaTime)
    {
        //if (HasBeenUpdatedThisFrame) return;
        if (LastUpdatedInTick == Globals.tickOscillator) return;
        switch (Material)
        {
            case EMaterial.SAND:
                MaterialPool.Sand.Update(this);
                break;
            case EMaterial.WATER:
                MaterialPool.Water.Update(this);
                break;
        }
    }

    public void Move(int x, int y)
    {
        var destinationCell = MapController.GetCellFromMapBuffer(x, y);
        destinationCell.Material = Material;
        destinationCell.Lifetime = Lifetime;
        destinationCell.Velocity = Velocity;
        //destinationCell.HasBeenUpdatedThisFrame = true;
        destinationCell.LastUpdatedInTick = Globals.tickOscillator;
        //When cell moved, set defaults at this Cell`s position
        Renderer.DrawCell(new Vector2I(x, y), Material); // draw in new position
        SetDefaults();
    }

    public void Move(Vector2I newPosition)
    {
        Move(newPosition.X, newPosition.Y);
    }

    public void Swap(int x, int y)
    {
        var destinationCell = MapController.GetCellFromMapBuffer(x, y);
        var tempMaterial = Material;
        var tempLifetime = Lifetime;
        var tempVelocity = Velocity;
        var tempTemperature = Temperature;
        var tempPositionOffset = PositionOffset;
        var tempLastUpdatedInTick = LastUpdatedInTick;

        Material = destinationCell.Material;
        Lifetime = destinationCell.Lifetime;
        Velocity = destinationCell.Velocity;
        Temperature = destinationCell.Temperature;
        PositionOffset = destinationCell.PositionOffset;
        LastUpdatedInTick = destinationCell.LastUpdatedInTick;

        destinationCell.Material = tempMaterial;
        destinationCell.Lifetime = tempLifetime;
        destinationCell.Velocity = tempVelocity;
        destinationCell.Temperature = tempTemperature;
        destinationCell.PositionOffset = tempPositionOffset;
        destinationCell.LastUpdatedInTick = tempLastUpdatedInTick;


        Renderer.DrawCell(destinationCell.ConstPosition, destinationCell.Material); // draw in new position
        Renderer.DrawCell(ConstPosition, Material); // draw in new position
    }

    public void Swap(Vector2I position)
    {
        Swap(position.X, position.Y);
    }

    private void SetDefaults()
    {
        Material = MaterialPool.Vacuum.Material;
        Lifetime = MaterialPool.Vacuum.Defaults.Lifetime;
        Velocity = MaterialPool.Vacuum.Defaults.Velocity;
        //HasBeenUpdatedThisFrame = false;
        LastUpdatedInTick = !Globals.tickOscillator;

        Renderer.DrawCell(new Vector2I(ConstPosition.X, ConstPosition.Y), MaterialPool.Vacuum.Material);
    }
    
}