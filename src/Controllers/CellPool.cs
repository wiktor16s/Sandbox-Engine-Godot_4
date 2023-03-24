using System.Linq.Expressions;
using Godot;
using SandboxEngine.Materials;
using SandboxEngine.Materials.Solid.Movable;

namespace SandboxEngine.Controllers;

/// <summary>
///     Controller declaring all Cells that will be implemented into the engine
/// </summary>
public static class CellPool
{
    public static readonly Sand Sand = new(
        EMaterial.SAND,
        new Color(255, 255, 0),
        4,
        1700,
        Globals.NeverFreezeValue,
        1,
        EMaterial.SAND,
        EMaterial.SAND,
        new Element.Defaults(20, new Vector2I(0,0), 0)
    );

    public static readonly Sand Vacuum = new(
        EMaterial.VACUUM,
        new Color(0, 0, 0),
        0,
        Globals.NeverBurnValue,
        Globals.NeverFreezeValue,
        0,
        EMaterial.VACUUM,
        EMaterial.VACUUM,
        new Element.Defaults(20, new Vector2I(0,0), 0)
    );
}