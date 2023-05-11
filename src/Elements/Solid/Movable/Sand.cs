using System;
using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Map;

namespace SandboxEngine.Elements.Solid.Movable;

public class Sand : Element
{
    public Sand(
        EMaterial id,
        Color color,
        int flashPoint,
        int freezingPoint,
        uint caloricValue,
        EMaterial afterFreezingTransformation,
        EMaterial afterBurningTransformation,
        DefaultValues defaultValues
    ) : base(id, color, flashPoint, freezingPoint, caloricValue, afterFreezingTransformation,
        afterBurningTransformation, defaultValues, ESubstance.SOLID)
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

    private bool IsVerticalVelocity(Cell cell)
    {
        return cell.Velocity.Y >= 1;
    }

    public override void Update(Cell cell)
    {
        if (!ShouldBeUpdated(cell)) return;
        cell.LastUpdatedInTick = Globals.tickOscillator;

        if (IsVerticalVelocity(cell))
        {
            var freeCellsDown = cell.CheckFreeCells((int)cell.Velocity.Y, Vector2I.Down);
            if (CanFallAllWayDown(cell, freeCellsDown))
            {
                FallAllWayDown(cell, freeCellsDown);
                return;
            }

            if (CanFallNotAllWayDown(cell, freeCellsDown))
            {
                FallNotAllWayDown(cell, freeCellsDown);
                return;
            }

            if (IsOnGround(freeCellsDown))
            {
                Convert_Y_To_X_Velocity(cell);

                if (IsHorizontalVelocity(cell))
                {
                    HandleHorizontalVelocity(cell);
                }

                return;
            }

            GD.Print("Unhandled vertical velocity");
        }
        else
        {
            var freeCellsDown = cell.CheckFreeCells(1, Vector2I.Down);
            if (CanStartFalling(freeCellsDown))
            {
                StartFalling(cell);
                if (IsHorizontalVelocity(cell))
                {
                    HandleDiagonalVelocity(cell);
                }
                else
                {
                    cell.Swap(cell.ConstPosition.X + (int)cell.Velocity.X, cell.ConstPosition.Y + (int)cell.Velocity.Y);
                    cell.Velocity.Y += Globals.Gravitation;
                }
            }
            else
            {
                var freeCellsLeftDown = cell.CheckFreeCells(1, Vector2I.Left + Vector2I.Down);
                var freeCellsRightDown = cell.CheckFreeCells(1, Vector2I.Right + Vector2I.Down);

                if (CanFallDiagonal(freeCellsLeftDown, freeCellsRightDown))
                {
                    HandleDiagonal(cell, freeCellsLeftDown, freeCellsRightDown);
                    return;
                }

                if (IsFluid(cell))
                {
                    HandleFluid(cell);
                }
                else
                {
                    cell.Velocity.X = 0;
                    cell.Velocity.Y = 0;
                    cell.IsFalling = false;
                }
            }
        }
    }

    private void Convert_Y_To_X_Velocity(Cell cell)
    {
        if (cell.Velocity.Y <= 0) return;

        var randomNum = GD.Randf() / 3 + 1;
        cell.Velocity.X = Utils.GetRandomBool()
            ? Math.Max((int)(cell.Velocity.Y / 3) * randomNum, 1f)
            : -Math.Max((int)(cell.Velocity.Y / 3) * randomNum, 1f);
        cell.Velocity.Y = 0;
    }

    private bool CanFallAllWayDown(Cell cell, int freeCellsDown)
    {
        return freeCellsDown == (int)cell.Velocity.Y;
    }

    private bool CanFallNotAllWayDown(Cell cell, int freeCellsDown)
    {
        return freeCellsDown > 0 && freeCellsDown < (int)cell.Velocity.Y;
    }

    private bool IsOnGround(int freeCellsDown)
    {
        return freeCellsDown == 0;
    }

    private bool CanStartFalling(int freeCellsDown)
    {
        return freeCellsDown > 0;
    }

    private void FallAllWayDown(Cell cell, int freeCellsDown)
    {
        cell.Velocity.Y += Globals.Gravitation;
        cell.Swap(cell.ConstPosition.X, cell.ConstPosition.Y + freeCellsDown);
    }

    private void FallNotAllWayDown(Cell cell, int freeCellsDown)
    {
        cell.Swap(cell.ConstPosition.X, cell.ConstPosition.Y + freeCellsDown);
    }

    private void StartFalling(Cell cell)
    {
        cell.Velocity.Y = 1;
    }

    private bool IsHorizontalVelocity(Cell cell)
    {
        return Math.Abs(cell.Velocity.X) >= 0;
    }

    private void HandleHorizontalVelocity(Cell cell)
    {
        var freeCellsLeft = cell.CheckFreeCells(Math.Abs((int)cell.Velocity.X), Vector2I.Left);
        var freeCellsRight = cell.CheckFreeCells(Math.Abs((int)cell.Velocity.X), Vector2I.Right);
        if (freeCellsLeft < 1 && freeCellsRight < 1)
        {
            cell.Velocity.X = 0;
            cell.Velocity.Y = 0;
            return;
        }

        if (freeCellsLeft > 0 && freeCellsRight > 0)
        {
            if (Utils.GetRandomBool())
            {
                freeCellsLeft = 0;
            }
            else
            {
                freeCellsRight = 0;
            }
        }

        if (freeCellsLeft > 0)
        {
            cell.Swap(cell.ConstPosition.X - freeCellsLeft, cell.ConstPosition.Y);
            cell.Velocity = Vector2.Zero;
        }

        if (freeCellsRight > 0)
        {
            cell.Swap(cell.ConstPosition.X + freeCellsRight, cell.ConstPosition.Y);
            cell.Velocity = Vector2.Zero;
        }
    }

    private void HandleDiagonalVelocity(Cell cell)
    {
        var freeCellsLeftDown = 0;
        var freeCellsRightDown = 0;

        if (cell.Velocity.X > 0)
            freeCellsRightDown = cell.CheckFreeCells((int)cell.Velocity.X, Vector2I.Right + Vector2I.Down);
        if (cell.Velocity.X < 0)
            freeCellsLeftDown = cell.CheckFreeCells((int)cell.Velocity.X, Vector2I.Left + Vector2I.Down);

        if (freeCellsLeftDown == 0 && freeCellsRightDown == 0)
        {
            cell.Velocity.X = 0;
            return;
        }

        if (freeCellsLeftDown > 0 && freeCellsRightDown > 0)
        {
            if (Utils.GetRandomBool())
            {
                freeCellsLeftDown = 0;
            }
            else
            {
                freeCellsRightDown = 0;
            }
        }

        if (freeCellsLeftDown > 0)
        {
            cell.Velocity.Y += Globals.Gravitation;
            cell.Velocity.X += 1;
            cell.Swap(cell.ConstPosition.X - freeCellsLeftDown, cell.ConstPosition.Y + freeCellsLeftDown);
        }

        if (freeCellsRightDown > 0)
        {
            cell.Velocity.Y += Globals.Gravitation;
            cell.Velocity.X += 1;
            cell.Swap(cell.ConstPosition.X + freeCellsLeftDown, cell.ConstPosition.Y + freeCellsLeftDown);
        }
    }

    public bool CanFallDiagonal(int freeCellsLeftDown, int freeCellsRightDown)
    {
        return freeCellsLeftDown > 0 || freeCellsRightDown > 0;
    }

    private void HandleDiagonal(Cell cell, int freeCellsLeftDown, int freeCellsRightDown)
    {
        if (freeCellsLeftDown > 0 && freeCellsRightDown > 0)
        {
            if (Utils.GetRandomBool())
            {
                freeCellsLeftDown = 0;
            }
            else
            {
                freeCellsRightDown = 0;
            }
        }

        if (freeCellsLeftDown > 0)
        {
            cell.Velocity.X -= 3;
            cell.Velocity.Y += 2;
            cell.Swap(cell.ConstPosition.X - freeCellsLeftDown, cell.ConstPosition.Y + freeCellsLeftDown);
        }
        else if (freeCellsRightDown > 0)
        {
            cell.Velocity.X += 3;
            cell.Velocity.Y += 2;
            cell.Swap(cell.ConstPosition.X + freeCellsRightDown, cell.ConstPosition.Y + freeCellsRightDown);
        }
    }

    private bool IsFluid(Cell cell)
    {
        return MaterialPool.GetByMaterial(cell.Material).Substance is ESubstance.FLUID;
    }

    private void HandleFluid(Cell cel)
    {
        // there are fluids
    }
}