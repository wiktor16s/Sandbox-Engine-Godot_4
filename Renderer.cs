using System;
using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Elements;
using SandboxEngine.Map;

namespace SandboxEngine;

public partial class Renderer : Sprite2D
{
    public Cell[]       _mapBuffer;  // array of cells todo move to self controller (performance impact)
    public Image        _mapImage;   // blob of image data
    public ImageTexture _mapTexture; // texture of image data

    public Vector2I _rendererIndex;
    public bool     IsActive           = true;
    public bool     LocalTickOscilator = true;
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
        cell.IsFalling = true;
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
        return position.X >= 0                    &&
               position.X < Globals.MapChunkWidth &&
               position.Y >= 0                    &&
               position.Y < Globals.MapChunkHeight;
    }

    public Vector2I NormalizePosition(Vector2I position)
    {
        // if (position.Y > Globals.MapChunkHeight - 1)
        // {
        //     position.Y = Globals.MapChunkHeight - 1;
        // }
        //
        // if (position.Y < 0)
        // {
        //     position.Y = 0;
        // }

        return position;
    }

    public Cell GetCellFromMapBuffer(Vector2I cellPosition)
    {
        var renderer = this;
        var position = cellPosition;

        if (!InBounds(cellPosition))
        {
            renderer = RenderManager.GetRendererByRelativePosition(cellPosition, this);
            position = RenderManager.GetOffsetOfRelativePosition(cellPosition);
        }

        try
        {
            return renderer._mapBuffer[ComputeIndex(position)];
        }
        catch (Exception e)
        {
            GD.Print(e);

            throw;
        }
    }

    public void SetIsFallingOnPath(Vector2I pos1, Vector2I pos2)
    {
        //todo optimalize this for god sake...!
        var path = Utils.GetShortestPathBetweenTwoCells(pos1, pos2, this);
        //GD.Print($"Pos1: {pos1} Pos2: {pos2} Path: {path}");
        foreach (var position in path)
        {
            var thisCellRenderer      = RenderManager.GetRendererByRelativePosition(position, this);
            var thisCellFixedPosition = RenderManager.GetOffsetOfRelativePosition(position);
            SetIsFallingAroundPosition(pos1);
            thisCellRenderer.SetIsFallingAroundPosition(thisCellFixedPosition);
            thisCellRenderer.SetIsFallingAroundPosition(pos2);
        }
    }

    public void SetIsFallingInSpecificPosition(Vector2I position)
    {
        if (RenderManager.IsPositionInAnyChunkBound(this, position))
        {
            var cellUp = GetCellFromMapBuffer(position);
            if (MaterialPool.GetByMaterial(cellUp.Material).Substance is not ESubstance.VACUUM)
            {
                cellUp.IsFalling = true;
            }
        }
    }

    public void SetIsFallingAroundPosition(Vector2I position)
    {
        SetIsFallingInSpecificPosition(position + Vector2I.Up);
        SetIsFallingInSpecificPosition(position + Vector2I.Down);
        SetIsFallingInSpecificPosition(position + Vector2I.Left);
        SetIsFallingInSpecificPosition(position + Vector2I.Right);
        // //
        // SetIsFallingInSpecificPosition(position + Vector2I.Up   + Vector2I.Right);
        // SetIsFallingInSpecificPosition(position + Vector2I.Up   + Vector2I.Left);
        // SetIsFallingInSpecificPosition(position + Vector2I.Down + Vector2I.Right);
        // SetIsFallingInSpecificPosition(position + Vector2I.Down + Vector2I.Left);
    }

    public void UpdateAll()
    {
        LocalTickOscilator = !LocalTickOscilator;
        for (var i = 0; i < Globals.MapChunkHeight; i++)
            if (i % 2 == 0)
                for (var j = 0; j < Globals.MapChunkWidth; j++)
                {
                    _mapBuffer[i * Globals.MapChunkWidth + j].Update(0f);
                }

            else
                for (var j = Globals.MapChunkWidth - 1; j >= 0; j--)
                    _mapBuffer[i * Globals.MapChunkWidth + j].Update(0f);
    }
}