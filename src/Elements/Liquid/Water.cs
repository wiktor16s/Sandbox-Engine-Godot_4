using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Map;

namespace SandboxEngine.Elements.Liquid;

public class Water : Element
{
    public int L = 0;
    public int R = 0;

    public Water(
        EMaterial material,
        Color color,
        Properties properties
    ) : base(material, ESubstance.FLUID, color, properties)
    {
        // constructor
    }

    public override void Update(Cell cell)
    {
        //cell.HasBeenUpdatedThisFrame = true;
        cell.LastUpdatedInTick = Globals.tickOscillator;

        var freeCellsDown = 0;
        var freeSpaceOnLeftDown = false;
        var freeSpaceOnRightDown = false;
        var freeSpaceOnLeft = false;
        var freeSpaceOnRight = false;

        for (var i = 1; i < Globals.Gravitation + 1; i++)
            // var nextCellMaterial = MapController.GetCellMaterialAt(cell.ConstPosition.X, cell.ConstPosition.Y);
            // var nextElement = CellPool.GetByMaterial(nextCellMaterial);
            if (
                MapController.InBounds(cell.ConstPosition.X, cell.ConstPosition.Y + i) &&
                MapController.GetCellFromMapBuffer(cell.ConstPosition.X, cell.ConstPosition.Y + i).Material ==
                EMaterial.VACUUM
            )
                freeCellsDown = i;
            else
                break;


        var leftFromThisCell = new Vector2I(cell.ConstPosition.X - 1, cell.ConstPosition.Y);
        var leftDownFromThisCell = new Vector2I(cell.ConstPosition.X - 1, cell.ConstPosition.Y + 1);
        var rightFromThisCell = new Vector2I(cell.ConstPosition.X + 1, cell.ConstPosition.Y);
        var rightDownFromThisCell = new Vector2I(cell.ConstPosition.X + 1, cell.ConstPosition.Y + 1);

        if (MapController.InBounds(rightFromThisCell))
            freeSpaceOnRight = MapController.GetCellFromMapBuffer(rightFromThisCell).Material == EMaterial.VACUUM;

        if (MapController.InBounds(leftFromThisCell))
            freeSpaceOnLeft = MapController.GetCellFromMapBuffer(leftFromThisCell).Material == EMaterial.VACUUM;

        if (MapController.InBounds(leftDownFromThisCell))
            freeSpaceOnLeftDown = MapController.GetCellFromMapBuffer(leftDownFromThisCell).Material == EMaterial.VACUUM;

        if (MapController.InBounds(rightDownFromThisCell))
            freeSpaceOnRightDown =
                MapController.GetCellFromMapBuffer(rightDownFromThisCell).Material == EMaterial.VACUUM;


        if (freeSpaceOnLeftDown && freeSpaceOnRightDown)
        {
            var randBool = Utils.GetRandomBool();

            freeSpaceOnLeftDown = !randBool;
            freeSpaceOnRightDown = randBool;
            freeSpaceOnLeft = false;
            freeSpaceOnRight = false;
        }
        else
        {
            if (freeSpaceOnRight && freeSpaceOnLeft)
            {
                var randBool = Utils.GetRandomBool();
                freeSpaceOnLeft = !randBool;
                freeSpaceOnRight = !freeSpaceOnLeft;
            }
        }

        if (freeCellsDown > 0)
        {
            cell.Velocity += new Vector2I(0, 1);
            cell.Move(cell.ConstPosition.X, cell.ConstPosition.Y + freeCellsDown);
            Renderer.DrawCell(new Vector2I(cell.ConstPosition.X, cell.ConstPosition.Y + freeCellsDown), Material);
        }
        else if (freeSpaceOnLeftDown)
        {
            cell.Move(leftDownFromThisCell);
            Renderer.DrawCell(leftDownFromThisCell, Material);
        }
        else if (freeSpaceOnRightDown)
        {
            cell.Move(rightDownFromThisCell);
            Renderer.DrawCell(rightDownFromThisCell, Material);
        }
        else if (freeSpaceOnRight)
        {
            cell.Move(rightFromThisCell);
            Renderer.DrawCell(rightFromThisCell, Material);
        }

        else if (freeSpaceOnLeft)
        {
            cell.Move(leftFromThisCell);
            Renderer.DrawCell(leftFromThisCell, Material);
        }


        // GD.PrintRich($"R {R}");
        // GD.PrintRich($"L {L}");

        if (freeCellsDown > 0 || freeSpaceOnLeft || freeSpaceOnRight)
            Renderer.DrawCell(cell.ConstPosition,
                MaterialPool.Vacuum.Material); //todo fix! apply swapping based on density
    }
}