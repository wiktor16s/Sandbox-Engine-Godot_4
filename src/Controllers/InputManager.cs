using Godot;
using SandboxEngine.Elements;

namespace SandboxEngine.Controllers;

public static class InputManager
{
    public static void UpdateMouseButtons(Viewport viewport) //todo
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            var mousePosition = (Vector2I)viewport.GetMousePosition().Floor();
            if (MouseInChunksBounds(mousePosition))
            {
                var hoverRenderer      = RenderManager.GetRendererBy2dIndex(RenderManager.GetRendererIndexByGlobalPosition(mousePosition));
                var mouseRenderPositon = ConvertGlobalToRendererPosition(mousePosition, hoverRenderer);

                hoverRenderer.GetCellFromMapBuffer(mouseRenderPositon).IsFalling = true;
                hoverRenderer.DrawCell(mouseRenderPositon, EMaterial.SAND);
            }
        }
    }

    public static bool MouseInChunksBounds(Vector2I mousePosition)
    {
        return mousePosition.X >= 0                                                                 &&
               mousePosition.X <= Globals.MapChunkWidth * Globals.GridWidth * Globals.RendererScale &&
               mousePosition.Y >= 0                                                                 &&
               mousePosition.Y <= Globals.MapChunkHeight * Globals.GridHeight * Globals.RendererScale;
    }

    public static Vector2I ConvertGlobalToRendererPosition(Vector2I globalPosition, Renderer renderer)
    {
        return new Vector2I(
            globalPosition.X / Globals.RendererScale - renderer._rendererIndex.X * Globals.MapChunkWidth,
            globalPosition.Y / Globals.RendererScale - renderer._rendererIndex.Y * Globals.MapChunkHeight
        );
    }
}