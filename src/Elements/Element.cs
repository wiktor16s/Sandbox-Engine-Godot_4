using Godot;
using SandboxEngine.Map;

namespace SandboxEngine.Elements;

public abstract class Element
{
    public readonly EMaterial AfterBurningTransformation;
    public readonly EMaterial AfterFreezingTransformation;
    public readonly uint CaloricValue;
    public readonly Color Color;
    public readonly DefaultValues Defaults;
    public readonly int FlashPoint;
    public readonly int FreezingPoint;
    public readonly EMaterial Material;

    /// <param name="id">Unique identifier or index of specific material</param>
    /// <param name="color">Color for this material</param>
    /// <param name="flashPoint">Temperature when this material catching fire</param>
    /// <param name="freezingPoint">Temperature when this material freezes</param>
    /// <param name="caloricValue">Amount of "fuel" determining how long this material will burn</param>
    /// <param name="afterFreezingTransformation">Determines which material it turns into when it freezes</param>
    /// <param name="afterBurningTransformation">Determines which material it will turn into when burned</param>
    /// <param name="defaults">Default values when cell is spawned</param>
    protected Element(
        EMaterial id,
        Color color,
        int flashPoint,
        int freezingPoint,
        uint caloricValue,
        EMaterial afterFreezingTransformation,
        EMaterial afterBurningTransformation,
        DefaultValues defaults
    )
    {
        Material = id;
        Color = color;
        FlashPoint = flashPoint;
        FreezingPoint = freezingPoint;
        CaloricValue = caloricValue;
        AfterFreezingTransformation = afterFreezingTransformation;
        AfterBurningTransformation = afterBurningTransformation;

        Defaults = defaults;
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
    UNKNOWN = 4
}