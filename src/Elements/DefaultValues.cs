using Godot;

namespace SandboxEngine.Elements;

public struct DefaultValues
{
    public int Temperature;
    public Vector2I Velocity;
    public uint Lifetime;
    public readonly float Density;
    public readonly float Stickiness;
    public readonly float Flowability;

    public DefaultValues(int temperature, Vector2I velocity, uint lifetime, float density, float stickiness)
    { 
        Temperature = temperature;
        Velocity = velocity;
        Lifetime = lifetime;
        Density = density;
        Stickiness = stickiness;
        Flowability = (2 * stickiness * density);
    }
}