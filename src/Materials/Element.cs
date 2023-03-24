using Godot;

namespace SandboxEngine.Materials;

public abstract class Element
{
    public readonly EMaterial AfterBurningTransformation;
    public readonly EMaterial AfterFreezingTransformation;
    public readonly uint CaloricValue;
    public readonly Color Color;
    public readonly short Density;
    public readonly int FlashPoint;
    public readonly int FreezingPoint;
    public readonly short Mass;
    public readonly EMaterial Material;
    public readonly Defaults DefaultValues;
    
    public struct Defaults
    {
        public int Temperature;
        public Vector2I Velocity;
        public uint Lifetime;

        public Defaults(int temperature, Vector2I velocity, uint lifetime)
        {
            Temperature = temperature;
            Velocity = velocity;
            Lifetime = lifetime;
        }
    }

    /// <param name="id">Unique identifier or index of specific material</param>
    /// <param name="color">Color for this material</param>
    /// <param name="density">Directly affects the Mass</param>
    /// <param name="flashPoint">Temperature when this material catching fire</param>
    /// <param name="freezingPoint">Temperature when this material freezes</param>
    /// <param name="caloricValue">Amount of "fuel" determining how long this material will burn</param>
    /// <param name="afterFreezingTransformation">Determines which material it turns into when it freezes</param>
    /// <param name="afterBurningTransformation">Determines which material it will turn into when burned</param>
    protected Element(
        EMaterial id,
        Color color,
        short density,
        int flashPoint,
        int freezingPoint,
        uint caloricValue,
        EMaterial afterFreezingTransformation,
        EMaterial afterBurningTransformation,
        Defaults defaultValues 
    )
    {
        Material = id;
        Color = color;
        Density = density;
        Mass = Density;
        FlashPoint = flashPoint;
        FreezingPoint = freezingPoint;
        CaloricValue = caloricValue;
        AfterFreezingTransformation = afterFreezingTransformation;
        AfterBurningTransformation = afterBurningTransformation;
        DefaultValues = defaultValues;
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

public class DefaultsForCell
{
    public int Temperature;
    public Vector2I Velocity;
    public uint Lifetime;
}