using System;
using Godot;
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
        afterBurningTransformation, defaultValues)
    {
        // constructor
    }

    public override void Update(Cell cell)
    {
        //cell.HasBeenUpdatedThisFrame = true;
        cell.LastUpdatedInTick = Globals.tickOscillator;

        //# ADD FORCES TO VELOCITY
        cell.Velocity.Y += Globals.Gravitation; // add gravitation acceleration to velocity

        //# UPDATE POSITION OFFSET WITH VELOCITY
        cell.PositionOffset += cell.Velocity;

        //# MOVE CELL
        if (Math.Round(cell.PositionOffset.Y) >= 1)
        {
            // move y+1
            // reset position offset
        }

        var freeCellsDown = 0;
        var freeSpaceOnLeftDown = false;
        var freeSpaceOnRightDown = false;


        freeCellsDown = cell.CheckFreeCellsForGravitation();
        freeSpaceOnLeftDown = cell.checkFreeCellsOnLeftDown();
        freeSpaceOnRightDown = cell.checkFreeCellsOnRightDown();

        if (freeSpaceOnLeftDown && freeSpaceOnRightDown)
        {
            var rand = GD.Randi() % 2 == 1;
            freeSpaceOnLeftDown = rand;
            freeSpaceOnRightDown = !rand;
        }

        if (freeCellsDown > 0)
        {
            cell.Velocity += new Vector2I(0, 1);
            cell.Swap(cell.ConstPosition.X, cell.ConstPosition.Y + freeCellsDown);
        }
        else if (freeSpaceOnLeftDown)
        {
            cell.Swap(cell.ConstPosition.X - 1, cell.ConstPosition.Y + 1);
        }
        else if (freeSpaceOnRightDown)
        {
            cell.Swap(cell.ConstPosition.X + 1, cell.ConstPosition.Y + 1);
        }
    }
}