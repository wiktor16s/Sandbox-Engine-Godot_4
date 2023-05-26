namespace SandboxEngine.Elements;

public struct Properties
{
    public readonly EMaterial AfterBurningTransformation;
    public readonly EMaterial AfterFreezingTransformation;
    public readonly float Bounciness;
    public readonly float FlashPoint;
    public readonly float Flowability;
    public readonly float FreezingPoint;
    public readonly float Density;
    public readonly float CaloricValue;

    /// <param name="flashPoint">Temperature when this material catching fire</param>
    /// <param name="freezingPoint">Temperature when this material freezes</param>
    /// <param name="caloricValue">Amount of "fuel" determining how long this material will burn</param>
    /// <param name="flowability">Substance flowability (stickiness to other particles )</param>
    /// <param name="density">Density of material</param>
    /// <param name="bounciness">Cell potential energy transformation to kinetic energy </param>
    /// <param name="afterFreezingTransformation">Determines which material it turns into when it freezes</param>
    /// <param name="afterBurningTransformation">Determines which material it will turn into when burned</param>
    public Properties(float density, float caloricValue, float bounciness, float flowability, float flashPoint,
        float freezingPoint,
        EMaterial afterFreezingTransformation, EMaterial afterBurningTransformation)
    {
        Density = density;
        CaloricValue = caloricValue;
        Bounciness = bounciness;
        Flowability = flowability;
        FlashPoint = flashPoint;
        FreezingPoint = freezingPoint;
        AfterFreezingTransformation = afterFreezingTransformation;
        AfterBurningTransformation = afterBurningTransformation;
    }
}