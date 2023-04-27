using Godot;
using SandboxEngine.Controllers;
using SandboxEngine.Elements;
using SandboxEngine.Map;

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
                return MaterialPool.Sand.Material;
            case (uint)EColors.BLUE:
                return MaterialPool.Water.Material;
            default:
                return MaterialPool.Vacuum.Material;
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
                    (MaterialPool.Sand.Color.R - LessRG) / 255,
                    (MaterialPool.Sand.Color.G - LessRG) / 255,
                    (MaterialPool.Sand.Color.B + MoreB) / 255
                );

                return newColor;
            }

            case EMaterial.WATER:
            {
                var LessB = GD.Randi() % 50;
                var MoreRG = GD.Randi() % 50;

                var newColor = new Color(
                    (MaterialPool.Water.Color.R + MoreRG) / 255,
                    (MaterialPool.Water.Color.G + MoreRG) / 255,
                    (MaterialPool.Water.Color.B - LessB) / 255
                );

                return newColor;
            }

            case EMaterial.VACUUM:
                return MaterialPool.Vacuum.Color;
            default:
                return new Color(255, 0, 255);
        }
    }


    public static void DrawCell(Vector2I position, EMaterial material)
    {
        MapController.GetCellFromMapBuffer(position.X, position.Y).SetMaterial(material);
        _mapImage.SetPixelv(position, GetColorByMaterial(material));
    }

    //! System Overrides

    public void UpdateMouseButtons()
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            var mousePosition = (Vector2I)GetViewport().GetMousePosition().Floor();
            if (MapController.InBounds(mousePosition.X, mousePosition.Y))
            {
                DrawCell(mousePosition, EMaterial.SAND);
            }
        }

        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            var mousePosition = (Vector2I)GetViewport().GetMousePosition().Floor();
            if (MapController.InBounds(mousePosition.X, mousePosition.Y))
            {
                DrawCell(mousePosition, EMaterial.WATER);
            }
        }
    }

    public override void _Ready()
    {
        LoadMapFromTexture();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Method intentionally left empty.
    }

    public override void _Process(double delta)
    {
        //var rand = GD.Randi() % 2 == 1;
        // if (Utils.Generator.Next(0, 2) == 0)
        // {
        //     A++;
        // }
        // else
        // {
        //     B++;
        // }

        //C++;
        //SUM += A - B;
        //GD.Print($"{SUM / C}");

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