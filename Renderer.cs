using System;
using Godot;
using SandboxEngine.Materials.Solid.Movable;

namespace SandboxEngine;

public partial class Renderer : Sprite2D
{
    public static Image _mapImage;
    public static ImageTexture _mapTexture;
    public int Height;
    public int Width;

    private void LoadMapFromTexture()
    {
        _mapImage = Texture.GetImage();
        _mapTexture = ImageTexture.CreateFromImage(_mapImage);
        Width = _mapImage.GetWidth();
        Height = _mapImage.GetHeight();

        MapController.Init(Width, Height);
        MapController.CopyImageToMap(_mapImage);
    }

    public static void DrawCell(Vector2I position, uint material)
    {
        MapController.GetCellAt(position.X, position.Y).SetMaterial(material);
        _mapImage.SetPixelv(position, new Color((uint)material));
    }

    //! System Overrides

    public void UpdateMouseButtons()
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            DrawCell((Vector2I)GetViewport().GetMousePosition().Floor(), Sand.Material);
        }
    }

    public override void _Ready()
    { 
        LoadMapFromTexture();
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }
    public override void _Process(double delta)
    {
        MapController.UpdateAll();
        _mapTexture.Update(_mapImage);
        Texture = _mapTexture;
        UpdateMouseButtons();
    }
}
