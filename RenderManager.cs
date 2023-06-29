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

                renderer.Texture   = ImageTexture.CreateFromImage(Image.LoadFromFile($"res://assets/Map/{GetChildren().Count}.bmp"));
                renderer.Scale     = new Vector2I(Globals.RendererScale, Globals.RendererScale);
                renderer.Name      = $"Renderer_{ComputeIndex(x, y)}";
                RenderChunks[x][y] = renderer;
                AddChild(renderer);
            }
        }
    }


    public override void _Process(double delta)
    {
        Globals.TickOscillator = !Globals.TickOscillator;

        InputManager.UpdateMouseButtons(GetViewport());
        foreach (var yChunks in RenderChunks)
        {
            foreach (var xChunk in yChunks)
            {
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
        if (position.X > Globals.GridWidth) throw new IndexOutOfRangeException("X Index of renderer is ot of range Globals GridWidth");

        if (position.Y > Globals.GridHeight) throw new IndexOutOfRangeException("Y Index of renderer is ot of range Globals GridHeight");

        return RenderChunks[position.X][position.Y];
    }

    public static Vector2I GetRendererIndexByMousePosition(Vector2I mousePosition)
    {
        return new Vector2I(
            (int)Math.Floor((double)(mousePosition.X / (Globals.MapChunkWidth  * Globals.RendererScale))),
            (int)Math.Floor((double)(mousePosition.Y / (Globals.MapChunkHeight * Globals.RendererScale)))
        );
    }

    public static Renderer GetRendererByMousePosition(Vector2I mousePosition)
    {
        var index = GetRendererIndexByMousePosition(mousePosition);
        return GetRendererBy2dIndex(index);
    }

    public static EMaterial GetMaterialByColor(Color color)
    {
        var colorI = color.ToRgba32();
        switch (colorI)
        {
            case 4294902015: //yellow
                return MaterialPool.Sand.Material;
            case 65535: //blue
                return MaterialPool.Water.Material;
            default:
                return MaterialPool.Vacuum.Material;
        }
    }

    public static Color GetColorByMaterial(EMaterial material, bool isDebug = false)
    {
        switch (material)
        {
            case EMaterial.SAND:
            {
                return Utils.Darken(MaterialPool.Sand.Color, 100);
            }

            case EMaterial.WATER:
            {
                return Utils.Darken(MaterialPool.Water.Color, 100);
            }

            case EMaterial.VACUUM:
                return MaterialPool.Vacuum.Color;
            default:
                return new Color(255, 0, 255);
        }
    }

    public static bool isChunkOnRight(Renderer actualRenderer)
    {
        return actualRenderer._rendererIndex.X < Globals.GridWidth - 1;
    }

    public static bool isChunkOnLeft(Renderer actualRenderer)
    {
        return actualRenderer._rendererIndex.X > 0;
    }

    public static bool isChunkOnAbove(Renderer actualRenderer)
    {
        return actualRenderer._rendererIndex.Y > 0;
    }

    public static bool isChunkOnBelow(Renderer actualRenderer)
    {
        return actualRenderer._rendererIndex.Y < Globals.GridHeight - 1;
    }
}