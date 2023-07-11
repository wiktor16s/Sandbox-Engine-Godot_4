using Godot;
using SandboxEngine.Map;

namespace SandboxEngine.Elements.Solid.Immovable;

public class Stone : Element
{
    public Stone(
        EMaterial  material,
        Color      color,
        Properties properties
    ) : base(material, ESubstance.STATIC, color, properties)
    {
        // constructor
    }

    public override void Update(Cell cell)
    {
        //cell.Move();
    }
}