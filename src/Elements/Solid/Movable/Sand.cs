using System;
using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Map;

namespace SandboxEngine.Elements.Solid.Movable;

public class Sand : Element
{
    public Sand(
        EMaterial material,
        Color color,
        Properties properties
    ) : base(material, ESubstance.SOLID, color, properties)
    {
        // constructor
    }

    private bool ShouldBeUpdated(Cell cell)
    {
        if (MaterialPool.GetByMaterial(cell.Material).Substance is not (ESubstance.FLUID or ESubstance.AIR
            or ESubstance.SOLID))
            return false;

        if (!cell.IsFalling) return false;
        return true;
    }


    public void ApplyGravity(Cell cell)
    {
        cell.Velocity += new Vector2I(0, 1);
    }

    public void ApplyAirResistance(Cell cell)
    {
        //cell.Velocity.X *= 0.8f;
    }

    public override void Update(Cell cell)
    {
        if (!ShouldBeUpdated(cell)) return;
        cell.LastUpdatedInTick = Globals.tickOscillator;

        ApplyGravity(cell);
        ApplyAirResistance(cell);
        var nextPosition = cell.ConstPosition + (Vector2I)cell.Velocity;
        var path = Utils.GetShortestPathBetweenTwoCells(cell.ConstPosition, nextPosition);
       
        var finalPosition = cell.ConstPosition;

        foreach (var pos in path)
            if (cell.CheckIsTargetPositionIsOccupiable(pos))
            {
                finalPosition = pos;
            }
            else
            {
                cell.Velocity.Y *= -0.3f;
                cell.Velocity.X += -Math.Sign(cell.Velocity.X) * cell.Velocity.Y * 0.5f;
                break;
            }


        if (finalPosition == cell.ConstPosition && cell.IsFalling)
        {
            var leftDiagonal = cell.ConstPosition + new Vector2I(-1, 1);
            var rightDiagonal = cell.ConstPosition + new Vector2I(1, 1);

            var canMoveLeftDiagonal = cell.CheckIsTargetPositionIsOccupiable(leftDiagonal);
            var canMoveRightDiagonal = cell.CheckIsTargetPositionIsOccupiable(rightDiagonal);

            if (canMoveLeftDiagonal && canMoveRightDiagonal)
                finalPosition = Utils.GetRandomBool() ? leftDiagonal : rightDiagonal;
            else if (canMoveLeftDiagonal)
                finalPosition = leftDiagonal;
            else if (canMoveRightDiagonal)
                finalPosition = rightDiagonal;
            else
            {
                cell.IsFalling = false;
                return;
            }

        }

        if (finalPosition == cell.ConstPosition && cell.IsFalling) {
            cell.IsFalling = false;
            return;
        }

        cell.Swap(finalPosition);
    }
}