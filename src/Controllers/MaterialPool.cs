using System;
using Godot;
using SandboxEngine.Elements;
using SandboxEngine.Elements.Gas;
using SandboxEngine.Elements.Liquid;
using SandboxEngine.Elements.None;
using SandboxEngine.Elements.Solid.Movable;

namespace SandboxEngine.Controllers;

/// <summary>
///     Controller declaring all Cells that will be implemented into the engine
/// </summary>
public static class MaterialPool
{
    public static readonly Sand Sand = new(
        EMaterial.SAND,
        new Color(255, 255, 0),
        new Properties(0.5f, 0, 0.3f, 0.4f, 1700f, -100f, EMaterial.SAND, EMaterial.SAND)
    );

    public static readonly Vacuum Vacuum = new(
        EMaterial.VACUUM,
        new Color(0, 0, 0),
        new Properties(0f, 0f, 0f, 0f, 0f, 0f, EMaterial.VACUUM, EMaterial.VACUUM)
    );

    public static readonly Water Water = new(
        EMaterial.WATER,
        new Color(0, 0, 255),
        new Properties(0.4f, 0f, 0.2f, 4f, 0f, 0f, EMaterial.WATER, EMaterial.WATER)
    );

    public static readonly Oxygen Oxygen = new(
        EMaterial.OXYGEN,
        new Color(220, 240, 255),
        new Properties(0.1f, 0f, 0.1f, 4f, 0f, 0f, EMaterial.OXYGEN, EMaterial.OXYGEN)
    );

    public static Element GetByMaterial(EMaterial material)
    {
        return material switch
        {
            EMaterial.SAND   => Sand,
            EMaterial.VACUUM => Vacuum,
            EMaterial.WATER  => Water,
            EMaterial.OXYGEN => Oxygen,
            _                => throw new ArgumentOutOfRangeException(nameof(material), material, null)
        };
    }
}