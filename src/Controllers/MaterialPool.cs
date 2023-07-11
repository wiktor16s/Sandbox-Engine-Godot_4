using System;
using System.Collections.Generic;
using Godot;
using SandboxEngine.Elements;
using SandboxEngine.Elements.Gas;
using SandboxEngine.Elements.Liquid;
using SandboxEngine.Elements.None;
using SandboxEngine.Elements.Solid.Immovable;
using SandboxEngine.Elements.Solid.Movable;

namespace SandboxEngine.Controllers;

/// <summary>
///     Controller declaring all Cells that will be implemented into the engine
/// </summary>
public static class MaterialPool
{
    public static Dictionary<uint, Element> ElementByColor = new();

    public static readonly Sand Sand = new(
        EMaterial.SAND,
        new Color(255, 255, 0),
        new Properties(0.5f, 0, 0.3f, 0.7f, 1700f, -100f, EMaterial.SAND, EMaterial.SAND)
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
        new Color(150, 200, 220),
        new Properties(0.1f, 0f, 0.1f, 10f, 0f, 0f, EMaterial.OXYGEN, EMaterial.OXYGEN)
    );

    public static readonly Stone Stone = new(
        EMaterial.STONE,
        new Color(50, 50, 50),
        new Properties(0.9f, 0, 0f, 0f, 1700f, -100f, EMaterial.STONE, EMaterial.STONE)
    );


    public static Element GetByColor(Color color)
    {
        var col = new Color(color.R8, color.G8, color.B8);

        if (col.ToRgba32() == Sand.Color.ToRgba32())
        {
            return Sand;
        }

        if (col.ToRgba32() == Vacuum.Color.ToRgba32())
        {
            return Vacuum;
        }

        if (col.ToRgba32() == Water.Color.ToRgba32())
        {
            return Water;
        }

        if (col.ToRgba32() == Oxygen.Color.ToRgba32())
        {
            return Oxygen;
        }

        if (col.ToRgba32() == Stone.Color.ToRgba32())
        {
            return Stone;
        }

        throw new Exception("Element not found");
    }

    public static Element GetByMaterial(EMaterial material)
    {
        return material switch
        {
            EMaterial.SAND   => Sand,
            EMaterial.VACUUM => Vacuum,
            EMaterial.WATER  => Water,
            EMaterial.OXYGEN => Oxygen,
            EMaterial.STONE  => Stone,
            _                => throw new ArgumentOutOfRangeException(nameof(material), material, null)
        };
    }
}