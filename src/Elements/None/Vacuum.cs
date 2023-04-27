using Godot;
using SandboxEngine.Map;

namespace SandboxEngine.Elements.None;

public class Vacuum : Element
{
    public Vacuum(EMaterial id, Color color, int flashPoint, int freezingPoint, uint caloricValue,
        EMaterial afterFreezingTransformation, EMaterial afterBurningTransformation, DefaultValues defaults) : base(id,
        color, flashPoint, freezingPoint, caloricValue, afterFreezingTransformation, afterBurningTransformation,
        defaults, ESubstance.VACUUM)
    {
    }

    public override void Update(Cell cell)
    {
        // Do nothing
    }
}