using Godot;
using SandboxEngine.Map;

namespace SandboxEngine.Elements.Liquid;

public class Water : Element
{
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
        cell.Move();
    }
}