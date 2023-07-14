using System;
using Godot;

namespace SandboxEngine.Scenes;

public partial class MapGenerator : Sprite2D
{
    private Image _image;
    private int   _mapHeight = 1024;
    private int   _mapWidth  = 1024;
    private float _offset;

    public override void _Ready()
    {
        Utils.Noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        Utils.Noise.Seed      = Utils.GetRandomInt(0, int.MaxValue);
        Scale                 = new Vector2I(2, 2);

        _image = Image.Create(_mapWidth, _mapHeight, false, Image.Format.Rgba8);

        for (var x = 0; x < _mapWidth; x++)
        {
            var noise  = Utils.Noise.GetNoise1D(_offset);
            var yNoise = Utils.Noise.GetNoise1D(_offset) * _mapHeight / Utils.GetRandomInt(2, 3) + _mapHeight / 1.3f;
            //var yNoise = Utils.MapValue(noise, -1, 1, _mapHeight / 2f, _mapHeight);
            var ySin   = Math.Sin(_offset);
            var height = yNoise;
            GD.Print(height);
            for (var y = 0; y < _mapHeight; y++)
            {
                if (y < height)
                    _image.SetPixel(x, y, new Color(0, 0, 0));
                else
                    _image.SetPixel(x, y, new Color(255, 255, 0));
            }


            _offset += Utils.GetRandomFloat(0f, 1f);
        }

        Texture = ImageTexture.CreateFromImage(_image);
    }

    public override void _Process(double delta)
    {
    }


    private void UpdateTexture()
    {
    }
}

// image.SetPixel(x, y, new Color(255, 255, 0));