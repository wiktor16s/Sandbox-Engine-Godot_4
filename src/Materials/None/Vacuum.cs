using Godot;
using SandboxEngine.Controllers;

namespace SandboxEngine.Materials.Solid.Movable;

public class Vacuum : Element
{
    public Vacuum(
        EMaterial id,
        Color color,
        short density,
        int flashPoint,
        int freezingPoint,
        uint caloricValue,
        Element.Defaults defaultValues,
        EMaterial afterFreezingTransformation,
        EMaterial afterBurningTransformation
    ) : base(id, color, density, flashPoint, freezingPoint, caloricValue, afterFreezingTransformation,
        afterBurningTransformation, defaultValues)
    {
        // constructor
    }

    public override void Update(Cell cell)
    {
        // Do nothing
    }
}