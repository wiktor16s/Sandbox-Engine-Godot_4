using System;
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
    public int Temperature;
    public Vector2 Velocity;

    public Cell(int x, int y)
    {
        Material = EMaterial.VACUUM;
        ConstPosition = new Vector2I(x, y);
        Temperature = 0;
        Lifetime = 0;
        LastUpdatedInTick = Globals.tickOscillator;
        IsFalling = false;

        // 🟦🟩🟨⬛️
    }

    public void SetMaterial(EMaterial material) // todo change type to interface
    {
        Material = material;
    }

    public Properties GetProperties()
    {
        return MaterialPool.GetByMaterial(Material).Properties;
    }

    public Element GetElement()
    {
        return MaterialPool.GetByMaterial(Material);
    }

    public bool CheckIsTargetPositionIsOccupiable(Vector2I targetPosition)
    {
        if (!MapController.InBounds(targetPosition.X, targetPosition.Y))
            return false;
        var targetCell = MapController.GetCellFromMapBuffer(targetPosition.X, targetPosition.Y);
        var isThisCellMoreDense = targetCell.GetProperties().Density <
                                  GetProperties().Density;
        var thisCellIsDiffMaterialThanTargetCell = targetCell.Material != Material;
        return isThisCellMoreDense && thisCellIsDiffMaterialThanTargetCell;
    }
    
    public bool CheckIsTargetIsOccupied(Vector2I targetPosition)
    {
        if (!MapController.InBounds(targetPosition.X, targetPosition.Y))
            return false;
        var targetCell = MapController.GetCellFromMapBuffer(targetPosition.X, targetPosition.Y);
        return targetCell.Material != EMaterial.VACUUM; //todo fix for gases
    }


    public void Update(float tickDeltaTime)
    {
        if (LastUpdatedInTick == Globals.tickOscillator) return;
        switch (Material)
        {
            case EMaterial.SAND:
                MaterialPool.Sand.Update(this);
                break;
            case EMaterial.WATER:
                MaterialPool.Water.Update(this);
                break;
            case EMaterial.VACUUM:
                MaterialPool.Vacuum.Update(this);
                break;
        }
    }

    public void Swap(int x, int y)
    {
        var destinationCell = MapController.GetCellFromMapBuffer(x, y);
        var tempMaterial = Material;
        var tempLifetime = Lifetime;
        var tempVelocity = Velocity;
        var tempTemperature = Temperature;
        var tempLastUpdatedInTick = LastUpdatedInTick;
        var tempIsFalling = IsFalling;

        Material = destinationCell.Material;
        Lifetime = destinationCell.Lifetime;
        Velocity = destinationCell.Velocity;
        Temperature = destinationCell.Temperature;
        LastUpdatedInTick = destinationCell.LastUpdatedInTick;
        IsFalling = destinationCell.IsFalling;

        destinationCell.Material = tempMaterial;
        destinationCell.Lifetime = tempLifetime;
        destinationCell.Velocity = tempVelocity;
        destinationCell.Temperature = tempTemperature;
        destinationCell.LastUpdatedInTick = tempLastUpdatedInTick;
        destinationCell.IsFalling = tempIsFalling;

        SetIsFallingOnPath(ConstPosition, destinationCell.ConstPosition);

        Renderer.DrawCell(destinationCell.ConstPosition, destinationCell.Material); // draw in new position
        Renderer.DrawCell(ConstPosition, Material); // draw in new position
    }

    public void Swap(Vector2I position)
    {
        Swap(position.X, position.Y);
    }

    public void SetIsFallingOnPath(Vector2I pos1, Vector2I pos2)
    {
        //todo optimalize this for god sake...!
        var path = Utils.GetShortestPathBetweenTwoCells(pos1, pos2);
        foreach (var position in path)
        {
            SetIsFallingAroundPosition(pos1);
            SetIsFallingAroundPosition(position);
            SetIsFallingAroundPosition(pos2);
        }
    }

    public void SetIsFallingInSpecificPosition(Vector2I position)
    {
        if (MapController.InBounds(position))
        {
            var cellUp = MapController.GetCellFromMapBuffer(position);
            if (MaterialPool.GetByMaterial(cellUp.Material).Substance is not ESubstance.VACUUM) cellUp.IsFalling = true;
        }
    }

    public void SetIsFallingAroundPosition(Vector2I position)
    {
        SetIsFallingInSpecificPosition(position + Vector2I.Up);
        SetIsFallingInSpecificPosition(position + Vector2I.Down);
        SetIsFallingInSpecificPosition(position + Vector2I.Left);
        SetIsFallingInSpecificPosition(position + Vector2I.Right);

        // SetIsFallingInSpecificPosition(position + Vector2I.Up + Vector2I.Right);
        // SetIsFallingInSpecificPosition(position + Vector2I.Up + Vector2I.Left);
        // SetIsFallingInSpecificPosition(position + Vector2I.Down + Vector2I.Right);
        // SetIsFallingInSpecificPosition(position + Vector2I.Down + Vector2I.Left);
    }

    private void SetDefaults()
    {
        Material = MaterialPool.Vacuum.Material;
        Lifetime = 0;
        Velocity = Vector2I.Zero;
        LastUpdatedInTick = !Globals.tickOscillator;

        Renderer.DrawCell(new Vector2I(ConstPosition.X, ConstPosition.Y), MaterialPool.Vacuum.Material);
    }

    private bool ShouldBeUpdated()
    {
        if (GetElement().Substance is not (ESubstance.FLUID or ESubstance.AIR
            or ESubstance.SOLID))
            return false;

        if (!IsFalling) return false;
        return true;
    }


    public void ApplyGravity()
    {
        Velocity += Vector2.Down;
    }

    public void ApplyAirResistance()
    {
        Velocity.X *= 0.3f;
    }

    public void HandleBounce()
    {
        //Velocity.Y *= -GetProperties().Bounciness;
        if (Math.Abs(Velocity.Y * -GetProperties().Bounciness) > 2)
            Velocity.Y *= -GetProperties().Bounciness;
        else
            Velocity.Y = 2;
    }

    public void Move()
    {
        if (!ShouldBeUpdated()) return;
        LastUpdatedInTick = Globals.tickOscillator;

        ApplyGravity();
        ApplyAirResistance();
        var path = Utils.GetShortestPathBetweenTwoCells(ConstPosition, ConstPosition + (Vector2I)Velocity);

        var finalPosition = ConstPosition;

        foreach (var pos in path)
            if (CheckIsTargetPositionIsOccupiable(pos))
            {
                finalPosition = pos;
            }
            else
            {
                // todo interact with other cell on hit
                HandleBounce();
                break;
            }


        if (finalPosition == ConstPosition && IsFalling)
        {
            var leftDiagonal = ConstPosition + new Vector2I(-1, 1);
            var rightDiagonal = ConstPosition + new Vector2I(1, 1);

            var canMoveLeftDiagonal = CheckIsTargetPositionIsOccupiable(leftDiagonal);
            var canMoveRightDiagonal = CheckIsTargetPositionIsOccupiable(rightDiagonal);

            if ((canMoveLeftDiagonal || canMoveRightDiagonal) && GD.Randf() > GetProperties().Flowability &&
                CheckIsTargetIsOccupied(ConstPosition + Vector2I.Down)) // if dół
            {
                canMoveLeftDiagonal = false;
                canMoveRightDiagonal = false;
            }

            if (canMoveLeftDiagonal && canMoveRightDiagonal)
                finalPosition = Utils.GetRandomBool() ? leftDiagonal : rightDiagonal;
            else if (canMoveLeftDiagonal)
                finalPosition = leftDiagonal;
            else if (canMoveRightDiagonal)
                finalPosition = rightDiagonal;
            else
                IsFalling = false;
        }

        if (finalPosition != ConstPosition) Swap(finalPosition);
    }
}
// abcdefghijklmno
/*
 *     // if substance is air or fluid
                if (GetElement().Substance is ESubstance.AIR or ESubstance.FLUID)
                {
                    var left = ConstPosition + new Vector2I(-1, 0);
                    var right = ConstPosition + new Vector2I(1, 0);

                    var canMoveLeft = CheckIsTargetPositionIsOccupiable(left);
                    var canMoveRight = CheckIsTargetPositionIsOccupiable(right);

                    if (canMoveLeft && canMoveRight)
                        finalPosition = Utils.GetRandomBool() ? left : right;
                    else if (canMoveLeft)
                        finalPosition = left;
                    else if (canMoveRight) finalPosition = right;
                }
                else
                {
                    IsFalling = false;
                    return;
                }
 */