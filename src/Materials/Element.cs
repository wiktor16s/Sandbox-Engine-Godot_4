using System.Xml;
using Godot;

namespace SandboxEngine.Materials;

public abstract class Element
{
    public uint Color;
    public short Mass;
    public short Material;
    public short Density;

    ///<param name="id">Unique identifier or index of specific material</param>
    ///<param name="color">Color`s value for this material</param>
    ///<param name="density">Directly affects the Mass</param>
    protected Element(short id, uint color, short density)
    {
        Material = id;
        _color = color;
        Density = density;
        Mass = Density;
    }
    /// <summary>
    /// Main core logic of physic calculations for specific Cell`s Material
    /// In this method need to implement all desired behaviors such as: movement, velocity, flash-point, etc...
    /// </summary>
    /// <param name="cell">Reference to affected cell</param>
    public abstract void Update(Element cell);
}