using System;
using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Elements;

namespace SandboxEngine;

public partial class RenderManager : Node
{
    public static Renderer[][] RenderChunks;


    public override void _Ready()
    {
        RenderChunks = new Renderer[Globals.GridWidth][];

        for (var i = 0; i < Globals.GridWidth; i++) RenderChunks[i] = new Renderer[Globals.GridHeight];
        for (var x = 0; x < Globals.GridWidth; x++)
        {
            for (var y = 0; y < Globals.GridHeight; y++)
            {
                var rendererScene = GD.Load<PackedScene>("res://Renderer.tscn");
                var renderer      = rendererScene.Instantiate<Renderer>();
                renderer._rendererIndex = new Vector2I(x, y);
                renderer.Position = new Vector2I(
                    x * Globals.MapChunkWidth  * Globals.RendererScale,
                    y * Globals.MapChunkHeight * Globals.RendererScale);

                renderer.GlobalPosition = renderer.Position;
                GD.Print($"x: {renderer.Position.X} y: {renderer.Position.Y}");

                renderer.Texture = ImageTexture.CreateFromImage(Image.LoadFromFile($"res://assets/Map/{GetChildren().Count}.bmp"));
                //renderer.Texture   = ImageTexture.CreateFromImage(Image.LoadFromFile("res://assets/5x5.bmp"));
                renderer.Scale     = new Vector2I(Globals.RendererScale, Globals.RendererScale);
                renderer.Name      = $"Renderer_{ComputeIndex(x, y)}";
                RenderChunks[x][y] = renderer;
                AddChild(renderer);
            }
        }
    }


    public override void _Process(double delta)
    {
        InputManager.UpdateMouseButtons(GetViewport());
        foreach (var yChunks in RenderChunks)
        {
            foreach (var xChunk in yChunks)
            {
                if (!xChunk.IsActive) break;
                xChunk.UpdateAll();
                xChunk._mapTexture.Update(xChunk._mapImage);
                xChunk.Texture = xChunk._mapTexture;
            }
        }
    }

    public static int ComputeIndex(int x, int y)
    {
        return y * Globals.GridWidth + x;
    }

    public static Renderer GetRendererBy2dIndex(Vector2I position)
    {
        if (position.X >= Globals.GridWidth) throw new IndexOutOfRangeException("X Index of renderer is ot of range Globals GridWidth");
        if (position.Y >= Globals.GridHeight) throw new IndexOutOfRangeException("Y Index of renderer is ot of range Globals GridHeight");

        return RenderChunks[position.X][position.Y];
    }

    public static Vector2I GetRendererIndexByGlobalPosition(Vector2I globalPosition)
    {
        return new Vector2I(
            (int)Math.Floor((double)(globalPosition.X / (Globals.MapChunkWidth  * Globals.RendererScale))),
            (int)Math.Floor((double)(globalPosition.Y / (Globals.MapChunkHeight * Globals.RendererScale)))
        );
    }

    public static Renderer GetRendererByRelativePosition(Vector2I position, Renderer originRenderer)
    {
        try
        {
            var nextRendererIndex = originRenderer._rendererIndex;

            if (position.X < 0) nextRendererIndex.X                       -= 1;
            if (position.X >= Globals.MapChunkWidth) nextRendererIndex.X  += 1;
            if (position.Y < 0) nextRendererIndex.Y                       -= 1;
            if (position.Y >= Globals.MapChunkHeight) nextRendererIndex.Y += 1;

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

        if (position.X > Globals.MapChunkWidth - 1) // right 128 +
        {
            final.X = position.X - Globals.MapChunkWidth;
        }

        else if (position.X < 0)
        {
            final.X = Globals.MapChunkWidth - Math.Abs(position.X);
        }

        if (position.Y > Globals.MapChunkHeight - 1)
        {
            final.Y = position.Y - Globals.MapChunkHeight;
        }

        if (position.Y < 0)
        {
            final.Y = Globals.MapChunkHeight - Math.Abs(position.Y);
        }

        return final;
    }

    public static Color GetColorByMaterial(EMaterial material, bool isDebug = false)
    {
        switch (material)
        {
            case EMaterial.SAND:
            {
                return Utils.Darken(MaterialPool.Sand.Color, 50);
            }

            case EMaterial.WATER:
            {
                return Utils.Darken(MaterialPool.Water.Color, 50);
            }

            case EMaterial.OXYGEN:
                return Utils.Darken(MaterialPool.Oxygen.Color, 15);

            case EMaterial.STONE:
                return Utils.Darken(MaterialPool.Stone.Color, 10);

            case EMaterial.VACUUM:
                return MaterialPool.Vacuum.Color;
            default:
                return new Color(255, 0, 255);
        }
    }

    public static bool IsChunkInDirection(Renderer actualRenderer, Vector2I direction)
    {
        var nextIndex = actualRenderer._rendererIndex + direction;

        return nextIndex.X >= 0 && nextIndex.X < Globals.GridWidth &&
               nextIndex.Y >= 0 && nextIndex.Y < Globals.GridHeight;
    }

    public static bool IsPositionInAnyChunkBound(Renderer originRenderer, Vector2I position)
    {
        if (originRenderer._rendererIndex.Y >= Globals.GridHeight - 1 && position.Y > Globals.MapChunkHeight - 1) return false;
        if (originRenderer._rendererIndex.Y <= 0                      && position.Y < 0) return false;
        if (originRenderer._rendererIndex.X >= Globals.GridWidth - 1  && position.X > Globals.MapChunkWidth - 1) return false;
        if (originRenderer._rendererIndex.X <= 0                      && position.X < 0) return false;

        return true;
    }

    public static Vector2I NormalizePositionIfNotInAnyChunkBound(Renderer originRenderer, Vector2I position)
    {
        if (originRenderer._rendererIndex.Y >= Globals.GridHeight - 1 && position.Y > Globals.MapChunkHeight - 1)
        {
            position.Y = Globals.MapChunkHeight - 1;
        }

        if (originRenderer._rendererIndex.Y <= 0 && position.Y < 0)
        {
            position.Y = 0;
        }

        if (originRenderer._rendererIndex.X >= Globals.GridWidth - 1 && position.X > Globals.MapChunkWidth - 1)
        {
            position.X = Globals.MapChunkWidth - 1;
        }

        if (originRenderer._rendererIndex.X <= 0 && position.X < 0)
        {
            position.X = 0;
        }

        return position;
    }
}