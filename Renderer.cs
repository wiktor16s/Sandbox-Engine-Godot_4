using System;
using Godot;
using SandboxEngine.Elements;
using SandboxEngine.Map;

namespace SandboxEngine;

public partial class Renderer : Sprite2D
{
    public Cell[]       _mapBuffer;  // array of cells todo move to self controller (performance impact)
    public Image        _mapImage;   // blob of image data
    public ImageTexture _mapTexture; // texture of image data

    public Vector2I _rendererIndex;
    public bool     IsActive = true;
    public Vector2I Position;

    private void LoadMapFromTexture()
    {
        _mapImage   = Texture.GetImage();
        _mapTexture = ImageTexture.CreateFromImage(_mapImage);
        CopyImageToMap(_mapImage, this);
    }


    public void DrawCell(Vector2I position, EMaterial material)
    {
        var cell = GetCellFromMapBuffer(position);
        cell.SetMaterial(material);
        _mapImage.SetPixelv(position, RenderManager.GetColorByMaterial(material));
    }


    public override void _Ready()
    {
        Engine.MaxFps = Globals.MaxFps;
// init map buffer
        _mapBuffer = new Cell[Globals.MapChunkHeight * Globals.MapChunkWidth];

        for (var i = 0; i < _mapBuffer.Length; i++)
        {
            var position = ComputePosition(i, Globals.MapChunkWidth);
            _mapBuffer[i] = new Cell(position.X, position.Y, this);
        }
// end of init map buffer


        LoadMapFromTexture();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Method intentionally left empty.
    }

    public override void _Process(double delta)
    {
        if (!IsActive)
        {
        }
    }

    public void CopyImageToMap(Image imageTexture, Renderer renderer)
    {
        for (var i = 0; i < renderer._mapBuffer.Length; i++)
        {
            var coords = ComputePosition(i, Globals.MapChunkWidth);
            var color  = imageTexture.GetPixel(coords.X, coords.Y);
            renderer._mapBuffer[i].SetMaterial(RenderManager.GetMaterialByColor(color));
        }
    }

    public static int ComputeIndex(Vector2I cellPosition)
    {
        return cellPosition.Y * Globals.MapChunkWidth + cellPosition.X;
    }

    public Vector2I ComputePosition(int index, int width)
    {
        return new Vector2I(
            index % width,
            index / width
        );
    }

    public bool InBounds(Vector2I position)
    {
        // var chunkWidth  = Globals.MapChunkWidth  * Globals.RendererScale;
        // var chunkHeight = Globals.MapChunkHeight * Globals.RendererScale;
        // var chunkIndexX = _rendererIndex.X;
        // var chunkIndexY = _rendererIndex.Y;
        // return position.X >= chunkIndexX      * chunkWidth  &&
        //        position.X < (chunkIndexX + 1) * chunkWidth  &&
        //        position.Y >= chunkIndexY      * chunkHeight &&
        //        position.Y < (chunkIndexY + 1) * chunkHeight;
        return position.X >= 0                    &&
               position.X < Globals.MapChunkWidth &&
               position.Y >= 0                    &&
               position.Y < Globals.MapChunkHeight;
    }

    public Vector2I NormalizePosition(Vector2I position)
    {
        if (position.Y > Globals.MapChunkHeight - 1)
        {
            position.Y = Globals.MapChunkHeight - 1;
        }

        if (position.Y < 0)
        {
            position.Y = 0;
        }

        return position;
    }

    public Cell GetCellFromMapBuffer(Vector2I cellPosition)
    {
        if (!InBounds(cellPosition)) throw new ArgumentException("Trying to reach out of bounds " + cellPosition);
        try
        {
            return _mapBuffer[ComputeIndex(cellPosition)];
        }
        catch (Exception e)
        {
            GD.Print(e);
            GD.Print(cellPosition);
            throw;
        }
    }

    public void UpdateAll()
    {
        for (var i = 0; i < Globals.MapChunkHeight; i++)
            if (i % 2 == 0)
                for (var j = 0; j < Globals.MapChunkWidth; j++)
                    _mapBuffer[i * Globals.MapChunkWidth + j].Update(0f);
            else
                for (var j = Globals.MapChunkWidth - 1; j > 0; j--)
                    _mapBuffer[i * Globals.MapChunkWidth + j].Update(0f);
    }
}