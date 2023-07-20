using System;
using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Elements;
using SandboxEngine.Map;
using SandboxEngine.Utils;

namespace SandboxEngine;

public partial class RenderManager : Node
{
    public static readonly Renderer[][] Renderers     = new Renderer[Globals.GridRendererWidth][];
    public static          Cell[][]     AllChunksById = new Cell[Globals.AmountOfChunksInRenderer][];

    public override void _Ready()
    {
        CreateRenderers();
        ThreadManager.InitRenderThreads();
    }


    public void CreateRenderers()
    {
        for (var i = 0; i < Globals.GridRendererWidth; i++) Renderers[i] = new Renderer[Globals.GridRendererHeight];
        for (var x = 0; x < Globals.GridRendererWidth; x++)
        {
            for (var y = 0; y < Globals.GridRendererHeight; y++)
            {
                var rendererScene = GD.Load<PackedScene>("res://Renderer.tscn");
                var renderer      = rendererScene.Instantiate<Renderer>();
                renderer._rendererIndex = new Vector2I(x, y);
                renderer.Position = new Vector2I(
                    x * Globals.MapRendererWidth  * Globals.RendererScale,
                    y * Globals.MapRendererHeight * Globals.RendererScale);

                renderer.GlobalPosition = renderer.Position;
                GD.Print($"x: {renderer.Position.X} y: {renderer.Position.Y}");

                renderer.Texture = ImageTexture.CreateFromImage(Image.LoadFromFile($"res://assets/Map/{GetChildren().Count}.bmp"));
                renderer.Scale   = new Vector2I(Globals.RendererScale, Globals.RendererScale);
                renderer.Name    = $"Renderer_{ComputeIndex(x, y)}";
                Renderers[x][y]  = renderer;
                AddChild(renderer);
            }
        }
    }

    public override void _Process(double delta)
    {
        InputManager.UpdateMouseButtons(GetViewport());
        InputManager.UpdateKeyboard();
        ThreadManager.ChunksIteration();
    }

    public static int ComputeIndex(int x, int y)
    {
        return y * Globals.GridRendererWidth + x;
    }

    public static Renderer GetRendererByIndex(int index)
    {
        return GetRendererBy2dIndex(new Vector2I(
            index % Globals.GridRendererWidth,
            index / Globals.GridRendererHeight
        ));
    }

    public static Renderer GetRendererBy2dIndex(Vector2I position)
    {
        if (position.X >= Globals.GridRendererWidth) throw new IndexOutOfRangeException("X Index of renderer is ot of range Globals GridWidth");
        if (position.Y >= Globals.GridRendererHeight) throw new IndexOutOfRangeException("Y Index of renderer is ot of range Globals GridHeight");

        return Renderers[position.X][position.Y];
    }

    public static Vector2I GetRendererIndexByGlobalPosition(Vector2I globalPosition)
    {
        return new Vector2I(
            (int)Math.Floor((double)(globalPosition.X / (Globals.MapRendererWidth  * Globals.RendererScale))),
            (int)Math.Floor((double)(globalPosition.Y / (Globals.MapRendererHeight * Globals.RendererScale)))
        );
    }

    public static Renderer GetRendererByRelativePosition(Vector2I position, Renderer originRenderer)
    {
        try
        {
            var nextRendererIndex = originRenderer._rendererIndex;

            if (position.X < 0) nextRendererIndex.X                          -= 1;
            if (position.X >= Globals.MapRendererWidth) nextRendererIndex.X  += 1;
            if (position.Y < 0) nextRendererIndex.Y                          -= 1;
            if (position.Y >= Globals.MapRendererHeight) nextRendererIndex.Y += 1;

            return GetRendererBy2dIndex(nextRendererIndex);
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }


    public static Vector2I GetOffsetOfRelativePosition(Vector2I position) // position from -127 -> (without 0) -> 127
    {
        var final = position;

        if (position.X > Globals.MapRendererWidth - 1) // right 128 +
        {
            final.X = position.X - Globals.MapRendererWidth;
        }

        else if (position.X < 0)
        {
            final.X = Globals.MapRendererWidth - Math.Abs(position.X);
        }

        if (position.Y > Globals.MapRendererHeight - 1)
        {
            final.Y = position.Y - Globals.MapRendererHeight;
        }

        if (position.Y < 0)
        {
            final.Y = Globals.MapRendererHeight - Math.Abs(position.Y);
        }

        return final;
    }

    public static Color GetColorByMaterial(EMaterial material, bool isDebug = false)
    {
        switch (material)
        {
            case EMaterial.SAND:
            {
                return Tools.Darken(MaterialPool.Sand.Color, 150);
            }

            case EMaterial.WATER:
            {
                return Tools.Darken(MaterialPool.Water.Color, 50);
            }

            case EMaterial.OXYGEN:
                return Tools.Darken(MaterialPool.Oxygen.Color, 15);

            case EMaterial.STONE:
                return Tools.Darken(MaterialPool.Stone.Color, 10);

            case EMaterial.VACUUM:
                return MaterialPool.Vacuum.Color;
            default:
                return new Color(255, 0, 255);
        }
    }

    public static bool IsChunkInDirection(Renderer actualRenderer, Vector2I direction)
    {
        var nextIndex = actualRenderer._rendererIndex + direction;

        return nextIndex.X >= 0 && nextIndex.X < Globals.GridRendererWidth &&
               nextIndex.Y >= 0 && nextIndex.Y < Globals.GridRendererHeight;
    }

    public static bool IsPositionInAnyChunkBound(Renderer originRenderer, Vector2I position)
    {
        if (originRenderer._rendererIndex.Y >= Globals.GridRendererHeight - 1 && position.Y > Globals.MapRendererHeight - 1) return false;
        if (originRenderer._rendererIndex.Y <= 0                              && position.Y < 0) return false;
        if (originRenderer._rendererIndex.X >= Globals.GridRendererWidth - 1  && position.X > Globals.MapRendererWidth - 1) return false;
        if (originRenderer._rendererIndex.X <= 0                              && position.X < 0) return false;

        return true;
    }

    public static Vector2I NormalizePositionIfNotInAnyChunkBound(Renderer originRenderer, Vector2I position)
    {
        if (originRenderer._rendererIndex.Y >= Globals.GridRendererHeight - 1 && position.Y > Globals.MapRendererHeight - 1)
        {
            position.Y = Globals.MapRendererHeight - 1;
        }

        if (originRenderer._rendererIndex.Y <= 0 && position.Y < 0)
        {
            position.Y = 0;
        }

        if (originRenderer._rendererIndex.X >= Globals.GridRendererWidth - 1 && position.X > Globals.MapRendererWidth - 1)
        {
            position.X = Globals.MapRendererWidth - 1;
        }

        if (originRenderer._rendererIndex.X <= 0 && position.X < 0)
        {
            position.X = 0;
        }

        return position;
    }
}