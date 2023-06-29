using Godot;
using SandboxEngine.Map;

namespace SandboxEngine.Elements.None;

public class Vacuum : Element
{
    public Vacuum(EMaterial material,
        Color               color,
        Properties          properties
    ) : base(material, ESubstance.VACUUM, color, properties)
    {
    }

    public override void Update(Cell cell)
    {
        if (cell.IsFalling)
        {
            cell.IsFalling = false;
            // Renderer.DrawCell(cell.ConstPosition, cell.Material);
        }
    }
}