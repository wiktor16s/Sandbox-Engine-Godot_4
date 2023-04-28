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
        afterBurningTransformation, defaultValues, ESubstance.SOLID)
    {
        // constructor
    }


    public override void Update(Cell cell)
    {
        // is movable
        int freeCellsDown = 0;

        if (!cell.IsFalling) return;
        if (cell.Velocity.Y >= 1)
        {
            freeCellsDown = cell.CheckFreeCells((int)cell.Velocity.Y, Vector2I.Down);
        }

        // var freeCellsDown = cell.CheckFreeCellsForGravitation();
        // var freeSpaceOnLeftDown = cell.checkFreeCellsOnLeftDown();
        // var freeSpaceOnRightDown = cell.checkFreeCellsOnRightDown();
        //
        // cell.Swap(cell.ConstPosition.X + 1, cell.ConstPosition.Y + 1);
    }
}