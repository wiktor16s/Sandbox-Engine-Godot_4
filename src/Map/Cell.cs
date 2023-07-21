using System;
using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Elements;
using SandboxEngine.Utils;

namespace SandboxEngine.Map;

public class Cell
{
    public readonly Vector2I  ConstPosition;
    public          bool      IsFalling;
    public          bool      LastUpdatedInTick;
    public          uint      Lifetime;
    public          EMaterial Material;
    public          Renderer  ParentRenderer;
    public          int       Temperature;
    public          Vector2   Velocity;

    public Cell(int x, int y, Renderer parentRenderer)
    {
        Material          = EMaterial.VACUUM;
        ConstPosition     = new Vector2I(x, y);
        Temperature       = 0;
        Lifetime          = 0;
        LastUpdatedInTick = true;
        IsFalling         = false;
        ParentRenderer    = parentRenderer;

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
        var renderer = ParentRenderer;
        var position = targetPosition;

        if (!RenderManager.IsPositionInAnyChunkBound(ParentRenderer, position))
        {
            return false;
        }

        if (!ParentRenderer.InBounds(position))
        {
            renderer = RenderManager.GetRendererByRelativePosition(position, ParentRenderer);
            position = RenderManager.GetOffsetOfRelativePosition(position);
        }

        var targetCell                           = renderer.GetCellFromMapBuffer(position);
        var isThisCellMoreDense                  = targetCell.GetProperties().Density < GetProperties().Density;
        var thisCellIsDiffMaterialThanTargetCell = targetCell.Material                != Material;
        return isThisCellMoreDense && thisCellIsDiffMaterialThanTargetCell;
    }


    public void Update(double tickDeltaTime)
    {
        if (LastUpdatedInTick == ParentRenderer.LocalTickOscilator) return;
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
            case EMaterial.OXYGEN:
                MaterialPool.Oxygen.Update(this);
                break;
        }
    }

    public void Swap(Vector2I destinationPosition)
    {
        var destinationCell       = ParentRenderer.GetCellFromMapBuffer(destinationPosition);
        var tempMaterial          = Material;
        var tempLifetime          = Lifetime;
        var tempVelocity          = Velocity;
        var tempTemperature       = Temperature;
        var tempLastUpdatedInTick = LastUpdatedInTick;
        var tempIsFalling         = IsFalling;

        Material          = destinationCell.Material;
        Lifetime          = destinationCell.Lifetime;
        Velocity          = destinationCell.Velocity;
        Temperature       = destinationCell.Temperature;
        LastUpdatedInTick = destinationCell.LastUpdatedInTick;
        IsFalling         = destinationCell.IsFalling;

        destinationCell.Material          = tempMaterial;
        destinationCell.Lifetime          = tempLifetime;
        destinationCell.Velocity          = tempVelocity;
        destinationCell.Temperature       = tempTemperature;
        destinationCell.LastUpdatedInTick = tempLastUpdatedInTick;
        destinationCell.IsFalling         = tempIsFalling;

        ParentRenderer.SetIsFallingOnPath(ConstPosition, destinationPosition);

        destinationCell.ParentRenderer.DrawCell(destinationCell.ConstPosition, destinationCell.Material); // draw in new position
        ParentRenderer.DrawCell(ConstPosition, Material);                                                 // draw in new position
    }

    private bool ShouldBeUpdated()
    {
        if (GetElement().Substance is not (ESubstance.FLUID or ESubstance.GAS
            or ESubstance.SOLID))
            return false;

        if (!IsFalling) return false;

        return true;
    }

    public void CheckLimits()
    {
        // max speed for cell to not commit into another chunk rendered in the same time (multithreading race condition)
        // if (Velocity.Y > Globals.MapRendererHeight / (Globals.AmountOfChunksInRenderer / 2))
        //     Velocity.Y = Globals.MapRendererHeight / (Globals.AmountOfChunksInRenderer / 2);
        //
        // if (Velocity.X > Globals.MapRendererWidth / (Globals.AmountOfChunksInRenderer / 2))
        //     Velocity.X = Globals.MapRendererWidth / (Globals.AmountOfChunksInRenderer / 2);
        //

        if (Math.Abs(Velocity.Y) > 8) Velocity.Y = Velocity.Y > 0 ? 8 : -8;
        if (Math.Abs(Velocity.X) > 8) Velocity.Y = Velocity.Y > 0 ? 8 : -8;
    }

    public void ApplyGravity()
    {
        switch (GetElement().Substance)
        {
            case ESubstance.SOLID:
            case ESubstance.FLUID:
                Velocity += Vector2.Down;
                break;
            case ESubstance.GAS:
                Velocity = Vector2I.Zero;
                break;
        }
    }

    public void ApplyAirResistance()
    {
        if (GetElement().Substance == ESubstance.GAS)
        {
            Velocity.X = 0;
        }
        else
        {
            Velocity.X *= 0.3f;
        }
    }

    public void HandleBounce()
    {
        if (Math.Abs(Velocity.Y * -GetProperties().Bounciness) > 2)
            Velocity.Y *= -GetProperties().Bounciness;
        else
            Velocity.Y = 2;
    }

    public void Move()
    {
        if (!ShouldBeUpdated()) return;
        LastUpdatedInTick = ParentRenderer.LocalTickOscilator;
        if (Tools.GetRandomFloat(0, 1) > 0.2f && GetElement().Substance == ESubstance.GAS) // skip ticks for gases
        {
            return;
        }

        CheckLimits();
        ApplyGravity();
        ApplyAirResistance();
        var path = Tools.GetShortestPathBetweenTwoCells(ConstPosition, ConstPosition + (Vector2I)Velocity, ParentRenderer);
        // todo optimize heap allocation Allocated size: 236.0 MB

        var finalPosition = ConstPosition;

        foreach (var pos in path)
        {
            var fixedPosition = pos;
            if (!RenderManager.IsPositionInAnyChunkBound(ParentRenderer, pos))
            {
                fixedPosition = RenderManager.NormalizePositionIfNotInAnyChunkBound(ParentRenderer, pos);
            }

            if (CheckIsTargetPositionIsOccupiable(fixedPosition))
            {
                finalPosition = fixedPosition;
            }
            else
            {
                // todo interact with other cell on hit
                HandleBounce();
                break;
            }
        }


        if (finalPosition == ConstPosition && IsFalling)
        {
            var leftDiagonal  = ConstPosition + new Vector2I(-1, 1);
            var rightDiagonal = ConstPosition + new Vector2I(1,  1);

            var canMoveLeftDiagonal  = CheckIsTargetPositionIsOccupiable(leftDiagonal);
            var canMoveRightDiagonal = CheckIsTargetPositionIsOccupiable(rightDiagonal);

            if ((canMoveLeftDiagonal || canMoveRightDiagonal) && GD.Randf() > GetProperties().Flowability &&
                !CheckIsTargetPositionIsOccupiable(ConstPosition + Vector2I.Down)) // if down cell is occupied (cannot fall)
            {
                canMoveLeftDiagonal  = false;
                canMoveRightDiagonal = false;
            }

            if (canMoveLeftDiagonal && canMoveRightDiagonal)
            {
                finalPosition = Tools.GetRandomBool() ? leftDiagonal : rightDiagonal;
            }
            else if (canMoveLeftDiagonal)
            {
                finalPosition = leftDiagonal;
            }
            else if (canMoveRightDiagonal)
            {
                finalPosition = rightDiagonal;
            }
            // todo Liquid flow
            else if (GetElement().Substance is ESubstance.FLUID or ESubstance.GAS)
            {
                var left         = ConstPosition + Vector2I.Left;
                var right        = ConstPosition + Vector2I.Right;
                var canMoveLeft  = true;
                var canMoveRight = true;

                var value = Tools.GetRandomInt(1, (int)GetProperties().Flowability);

                for (var i = 0; i < value; i++)
                {
                    if (canMoveLeft)
                    {
                        left.X      -= i;
                        canMoveLeft =  CheckIsTargetPositionIsOccupiable(left);
                    }

                    if (canMoveRight)
                    {
                        right.X      += i;
                        canMoveRight =  CheckIsTargetPositionIsOccupiable(right);
                    }
                }

                if (canMoveLeft && canMoveRight)
                    finalPosition = Tools.GetRandomBool() ? left : right;
                else if (canMoveLeft)
                    finalPosition                    = left;
                else if (canMoveRight) finalPosition = right;
            }
            else
            {
                IsFalling = false;
            }
        }

        if (finalPosition != ConstPosition)
        {
            Swap(finalPosition);
        }
    }
}