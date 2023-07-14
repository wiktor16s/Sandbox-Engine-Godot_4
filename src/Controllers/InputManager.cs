using System.Collections.Generic;
using Godot;
using SandboxEngine.Elements;

namespace SandboxEngine.Controllers;

public static class InputManager
{
    public static EMaterial selectedMaterial = EMaterial.SAND;
    public static int       brushSize        = 1;

    public static void UpdateKeyboard()
    {
        if (Input.IsKeyPressed(Key.Key1)) selectedMaterial = EMaterial.SAND;
        if (Input.IsKeyPressed(Key.Key2)) selectedMaterial = EMaterial.WATER;
        if (Input.IsKeyPressed(Key.Key3)) selectedMaterial = EMaterial.STONE;
        if (Input.IsKeyPressed(Key.Key4)) selectedMaterial = EMaterial.OXYGEN;
        if (Input.IsKeyPressed(Key.Key5)) selectedMaterial = EMaterial.VACUUM;
    }

    public static void UpdateMouseButtons(Viewport viewport) //todo
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            var mousePosition = (Vector2I)viewport.GetMousePosition().Floor();
            var positions     = new List<Vector2I>();

            positions.Add(mousePosition + Vector2I.Up);
            positions.Add(mousePosition + Vector2I.Left);
            positions.Add(mousePosition + Vector2I.Right);
            positions.Add(mousePosition + Vector2I.Down);

            for (var i = 0; i < positions.Count; i++)
            {
                if (MouseInChunksBounds(positions[i]))
                {
                    var hoverRenderer      = RenderManager.GetRendererBy2dIndex(RenderManager.GetRendererIndexByGlobalPosition(positions[i]));
                    var mouseRenderPositon = ConvertGlobalToRendererPosition(positions[i], hoverRenderer);

                    hoverRenderer.GetCellFromMapBuffer(mouseRenderPositon).IsFalling = true;
                    hoverRenderer.DrawCell(mouseRenderPositon, selectedMaterial);
                    hoverRenderer.SetIsFallingAroundPosition(mouseRenderPositon);
                }
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