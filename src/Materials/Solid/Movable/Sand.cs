using Godot;
using SandboxEngine.Controllers;

namespace SandboxEngine.Materials.Solid.Movable;

public class Sand : Element
{
    public Sand(
        EMaterial id,
        Color color,
        short density,
        int flashPoint,
        int freezingPoint,
        uint caloricValue,
        EMaterial afterFreezingTransformation,
        EMaterial afterBurningTransformation,
        Element.Defaults defaultValues
    ) : base(id, color, density, flashPoint, freezingPoint, caloricValue, afterFreezingTransformation,
        afterBurningTransformation, defaultValues)
    {
        // constructor
    }

    public override void Update(Cell cell)
    {
        var down = 0;
        var left = false;
        var right = false;
        const int gravitation = 4;

        for (var i = 1; i < gravitation + 1; i++)
            if (MapController.IsEmpty(cell.Position.X, cell.Position.Y + i))
                down = i;
            else
                break;

        if (cell.Position.X - 1 >= 0) left = MapController.IsEmpty(cell.Position.X - 1, cell.Position.Y + 1);
        if (cell.Position.X + 1 < MapController.Height)
            right = MapController.IsEmpty(cell.Position.X + 1, cell.Position.Y + 1);

        if (left && right)
        {
            var rand = GD.Randi() % 2 == 1;
            left = rand;
            right = !rand;
        }

        if (down > 0)
        {
            cell.Velocity += new Vector2I(0, 1);
            cell.Move(cell.Position.X, cell.Position.Y + down);
            Renderer.DrawCell(new Vector2I(cell.Position.X, cell.Position.Y + down), Material);
        }
        else if (left)
        {
            cell.Move(cell.Position.X - 1, cell.Position.Y + 1);
            Renderer.DrawCell(new Vector2I(cell.Position.X - 1, cell.Position.Y + 1), Material);
        }
        else if (right)
        {
            cell.Move(cell.Position.X + 1, cell.Position.Y + 1);
            Renderer.DrawCell(new Vector2I(cell.Position.X + 1, cell.Position.Y + 1), Material);
        }

        if (down > 0 || left || right) Renderer.DrawCell(new Vector2I(cell.Position.X, cell.Position.Y), CellPool.Vacuum.Material); //todo fix! apply swapping
    }
}