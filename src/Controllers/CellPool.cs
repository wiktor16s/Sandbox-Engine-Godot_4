using SandboxEngine.Materials.Solid.Movable;

namespace SandboxEngine.Controllers;
/// <summary>
/// Controller declaring all Cells that will be implemented into the engine
/// </summary>
public static class CellPool
{
    public static Materials.Solid.Movable.Sand Sand = new Sand(0, 4294902015, 4);
}