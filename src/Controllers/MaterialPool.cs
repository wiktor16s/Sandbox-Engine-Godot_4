using System;
using Godot;
using SandboxEngine.Materials;
using SandboxEngine.Materials.None;
using SandboxEngine.Materials.Solid.Movable;

namespace SandboxEngine.Controllers;

/// <summary>
///     Controller declaring all Cells that will be implemented into the engine
/// </summary>
public static class MaterialPool
{
    public static readonly Sand Sand = new(
        EMaterial.SAND,
        new Color(255, 255, 0),
        1700,
        -100,
        1,
        EMaterial.SAND,
        EMaterial.SAND,
        new DefaultValues(20, new Vector2I(0, 0), 0, new Vector2I(0, 0), 50, 1)
    );

    public static readonly Vacuum Vacuum = new(
        EMaterial.VACUUM,
        new Color(0, 0, 0),
        Globals.NeverBurnValue,
        Globals.NeverFreezeValue,
        0,
        EMaterial.VACUUM,
        EMaterial.VACUUM,
        new DefaultValues(0, new Vector2I(0, 0), 0, new Vector2I(0, 0), short.MinValue, short.MinValue)
    );
    
    public static readonly Water Water = new(
        EMaterial.WATER,
        new Color(0, 0, 255),
        Globals.NeverBurnValue,
        Globals.NeverFreezeValue,
        0,
        EMaterial.WATER,
        EMaterial.WATER,
        new DefaultValues(0, new Vector2I(0, 0), 0, new Vector2I(0, 0), 10, 1)
    );

    public static Element GetByMaterial(EMaterial material)
    {
        return material switch
        {
            EMaterial.SAND => Sand,
            EMaterial.VACUUM => Vacuum,
            EMaterial.WATER => Water,
            _ => throw new ArgumentOutOfRangeException(nameof(material), material, null)
        };
    }
}