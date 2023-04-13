using Godot;

namespace SandboxEngine.Elements;

public struct DefaultValues
{
    public int Temperature;
    public Vector2I Velocity;
    public uint Lifetime;
    public Vector2 PositionOffset;
    public readonly short Density;
    public readonly short Mass;

    public DefaultValues(int temperature, Vector2I velocity, uint lifetime, Vector2I positionOffset, short density,
        short mass)
    {
        Temperature = temperature;
        Velocity = velocity;
        Lifetime = lifetime;
        PositionOffset = positionOffset;
        Density = density;
        Mass = (short)(density * mass);
    }
}