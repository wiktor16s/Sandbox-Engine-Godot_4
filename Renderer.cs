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

    public static Color GetColorByMaterial(EMaterial material, bool isDebug = false)
    {
        if (isDebug) return new Color(255, 0, 255);

        switch (material)
        {
            case EMaterial.SAND:
            {
                return Utils.Darken(MaterialPool.Sand.Color, 100);
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
        var cell = MapController.GetCellFromMapBuffer(position.X, position.Y);
        cell.SetMaterial(material);
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
                MapController.GetCellFromMapBuffer(mousePosition).IsFalling = true;
                DrawCell(mousePosition, EMaterial.SAND);
            }
        }

        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            var mousePosition = (Vector2I)GetViewport().GetMousePosition().Floor();
            if (MapController.InBounds(mousePosition.X, mousePosition.Y))
            {
                MapController.GetCellFromMapBuffer(mousePosition).IsFalling = true;
                DrawCell(mousePosition, EMaterial.WATER);
            }
        }

        if (Input.IsMouseButtonPressed(MouseButton.Middle))
        {
            var mousePosition = (Vector2I)GetViewport().GetMousePosition().Floor();
            if (MapController.InBounds(mousePosition.X, mousePosition.Y))
            {
                MapController.GetCellFromMapBuffer(mousePosition).IsFalling = true;
                DrawCell(mousePosition, EMaterial.OXYGEN);
            }
        }
    }

    public override void _Ready()
    {
        Engine.MaxFps = Globals.MaxFps;
        LoadMapFromTexture();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Method intentionally left empty.
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