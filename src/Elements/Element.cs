using Godot;
using SandboxEngine.Map;

namespace SandboxEngine.Elements;

public abstract class Element
{
    public readonly Color Color;
    public readonly EMaterial Material;
    public readonly Properties Properties;
    public readonly ESubstance Substance;


    /// <param name="material">Unique identifier or index of specific material</param>
    /// <param name="color">Color for this material</param>
    /// <param name="properties">Default values when cell is spawned</param>
    /// <param name="substance">Defines type of substance like fluid, solid static</param>
    protected Element(
        EMaterial material,
        ESubstance substance,
        Color color,
        Properties properties
    )
    {
        Material = material;
        Substance = substance;
        Color = color;
        Properties = properties;
    }

    /// <summary>
    ///     Main core logic of physic calculations for specific Cell`s Material
    ///     In this method need to implement all desired behaviors such as: movement, velocity, flash-point, etc...
    /// </summary>
    /// <param name="cell">Reference to affected cell</param>
    public abstract void Update(Cell cell);


    public uint GetDefaultUintColor()
    {
        return Color.ToRgba32();
    }
}

public enum EMaterial : short
{
    SAND = 0,
    STONE = 1,
    VACUUM = 2,
    WATER = 3,
    UNKNOWN = 4,
    OXYGEN = 5
}

public enum ESubstance : short
{
    SOLID = 0,
    FLUID = 1,
    GAS = 2,
    STATIC = 3,
    VACUUM = 4
}