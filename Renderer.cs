using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Materials;

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

    public static EMaterial GetMaterialByColor(Color color)
    {
        var colorI = color.ToRgba32();
        switch (colorI)
        {
            case (uint)EColors.YELLOW:
                return CellPool.Sand.Material;
            default:
                return CellPool.Vacuum.Material;
        }
    }

    public static Color GetColorByMaterial(EMaterial material)
    {
        switch (material)
        {
            case EMaterial.SAND:
            {
                var LessRG = GD.Randi() % 50;
                var MoreB = GD.Randi() % 50;

                var newColor = new Color(
                    (CellPool.Sand.Color.R - LessRG) / 255,
                    (CellPool.Sand.Color.G - LessRG) / 255,
                    (CellPool.Sand.Color.B + MoreB) / 255
                );

                return newColor;
            }

            case EMaterial.VACUUM:
                return CellPool.Vacuum.Color;
            default:
                return new Color(255, 0, 255);
        }
    }


    public static void DrawCell(Vector2I position, EMaterial material)
    {
        MapController.GetCellAt(position.X, position.Y).SetMaterial(material);
        _mapImage.SetPixelv(position, GetColorByMaterial(material));
    }

    //! System Overrides

    public void UpdateMouseButtons()
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
            DrawCell((Vector2I)GetViewport().GetMousePosition().Floor(), EMaterial.SAND);
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

public enum EColors : uint
{
    YELLOW = 4294902015,
    WHITE = 4294967295,
    BLACK = 255,
    BLUE = 65535,
    RED = 4278190335
}