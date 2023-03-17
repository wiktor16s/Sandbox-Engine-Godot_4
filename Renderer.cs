using Godot;

namespace SandboxEngine;

public partial class Renderer : Sprite2D
{
    private static Image _mapImage;
    private Texture _mapTexture;
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

    public static void DrawCell(Vector2I position, EMaterial material)
    {
        MapController.GetCellAt(position.X, position.Y).SetMaterial(material);
        _mapImage.SetPixelv(position, new Color((uint)material));
    }

    // //! System Overrides
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
            DrawCell((Vector2I)eventMouseMotion.Position.Floor(), EMaterial.SAND);
        // if (@event is InputEventMouseButton eventMouseButton)
        // {
        //     GD.Print(eventMouseMotion.Position);
        //     if (eventMouseButton.IsPressed())
        //     {
        //     }
        // }
    }

    public override void _Ready()
    {
        
        LoadMapFromTexture();
    }

    public override void _Process(double delta)
    {
        //GD.Print(1000 / delta);
        MapController.UpdateAll();
        Texture = ImageTexture.CreateFromImage(_mapImage);
    }
}