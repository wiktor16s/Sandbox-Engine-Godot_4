using System;
using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Elements;
using SandboxEngine.Map;
using SandboxEngine.Utils;

namespace SandboxEngine;

public partial class Renderer : Sprite2D
{
    private Cell[][]     _chunkOfCells; //[ChunkId][Cell]
    public  ChunkState   _chunkState = new(true, true, true, true);
    public  Cell[]       _mapBuffer;  // array of cells todo move to self controller (performance impact)
    public  Image        _mapImage;   // blob of image data
    public  ImageTexture _mapTexture; // texture of image data
    public  Vector2I     _rendererIndex;
    public  bool         IsActive           = false;
    public  bool         LocalTickOscilator = true;
    public  Vector2I     Position;

    private void LoadMapBufferFromTexture()
    {
        _mapImage   = Texture.GetImage();
        _mapTexture = ImageTexture.CreateFromImage(_mapImage);
        CopyImageToMap(_mapImage, this);
    }

    public void InitMapBuffer()
    {
        _mapBuffer = new Cell[Globals.MapRendererHeight * Globals.MapRendererWidth];
        for (var i = 0; i < _mapBuffer.Length; i++)
        {
            var position = ComputePosition(i, Globals.MapRendererWidth);
            _mapBuffer[i] = new Cell(position.X, position.Y, this);
        }
    }

    public Cell[] GetSpecificChunkOfCells(int id)
    {
        if (id > Globals.AmountOfChunksInRenderer - 1 || id < 0) throw new ArgumentOutOfRangeException($"Chunk id: {id} - is out of range ");
        return _chunkOfCells[id];
    }

    public void DivideMapBufferIntoChunks()
    {
        _chunkOfCells = Tools.SplitCellArrayIntoSquareChunks(_mapBuffer);
    }

    public void DrawCell(Vector2I position, EMaterial material, bool fall = true)
    {
        var cell = GetCellFromMapBuffer(position);
        cell.SetMaterial(material);
        cell.IsFalling = fall;
        _mapImage.SetPixelv(position, RenderManager.GetColorByMaterial(material));
    }


    public override void _Ready()
    {
        Engine.MaxFps = Globals.MaxFps;
        InitMapBuffer();
        LoadMapBufferFromTexture();
        DivideMapBufferIntoChunks();
    }

    public void CopyImageToMap(Image imageTexture, Renderer renderer)
    {
        for (var i = 0; i < renderer._mapBuffer.Length; i++)
        {
            var coords   = ComputePosition(i, Globals.MapRendererWidth);
            var color    = imageTexture.GetPixel(coords.X, coords.Y);
            var material = MaterialPool.GetByColor(color).Material;
            renderer._mapBuffer[i].SetMaterial(material);
            DrawCell(coords, material, false);
        }
    }

    public static int ComputeIndex(Vector2I cellPosition)
    {
        return cellPosition.Y * Globals.MapRendererWidth + cellPosition.X;
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
        return position.X >= 0                       &&
               position.X < Globals.MapRendererWidth &&
               position.Y >= 0                       &&
               position.Y < Globals.MapRendererHeight;
    }

    public Vector2I NormalizePosition(Vector2I position)
    {
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
        var path = Tools.GetShortestPathBetweenTwoCells(pos1, pos2, this);
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

    public void ProcessChunk(int chunkIndex)
    {
        for (var i = 0; i < Globals.MapRendererHeight / Globals.AmountOfChunksInRenderer * 2; i++)
            if (i % 2 == 0)
                for (var j = 0; j < Globals.MapRendererWidth / Globals.AmountOfChunksInRenderer * 2; j++)
                    _chunkOfCells[chunkIndex][i * Globals.MapRendererWidth / Globals.AmountOfChunksInRenderer * 2 + j].Update(0f);


            else
                for (var j = Globals.MapRendererWidth / Globals.AmountOfChunksInRenderer * 2 - 1; j >= 0; j--)
                    _chunkOfCells[chunkIndex][i * Globals.MapRendererWidth / Globals.AmountOfChunksInRenderer * 2 + j].Update(0f);
    }

    public void UpdateTexture()
    {
        LocalTickOscilator = !LocalTickOscilator;
        _mapTexture.Update(_mapImage);
        Texture = _mapTexture;
    }

    public struct ChunkState
    {
        public bool isActiveChunk0;
        public bool isActiveChunk1;
        public bool isActiveChunk2;
        public bool isActiveChunk3;

        public ChunkState(bool ch0, bool ch1, bool ch2, bool ch3)
        {
            isActiveChunk0 = ch0;
            isActiveChunk1 = ch1;
            isActiveChunk2 = ch2;
            isActiveChunk3 = ch3;
        }

        private void SetChunkState(int chunkId, bool state)
        {
            switch (chunkId)
            {
                case 0:
                    isActiveChunk0 = state;
                    break;
                case 1:
                    isActiveChunk1 = state;
                    break;
                case 2:
                    isActiveChunk2 = state;
                    break;
                case 3:
                    isActiveChunk3 = state;
                    break;
            }
        }
    }
}