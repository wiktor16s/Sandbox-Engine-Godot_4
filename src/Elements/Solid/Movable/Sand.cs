using Godot;
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

    public override void Update(Cell cell)
    {
        cell.Move();
    }
}