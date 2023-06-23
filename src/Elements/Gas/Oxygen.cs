using Godot;
using SandboxEngine.Map;

namespace SandboxEngine.Elements.Gas;

public class Oxygen : Element
{
    public Oxygen(
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