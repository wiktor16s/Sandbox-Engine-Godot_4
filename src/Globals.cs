namespace SandboxEngine;

public static class Globals
{
    public const  float Gravitation               = 1f;
    public static int   MaxFps                    = 60;
    public static int   MapChunkWidth             = 128;
    public static int   MapChunkHeight            = MapChunkWidth;
    public static int   AmountOfChunksPerRenderer = 4;
    public static int   GridWidth                 = 2;
    public static int   GridHeight                = 2;
    public static int   RendererScale             = 3;
    public static int   GridTotalRenderers        = GridWidth * GridHeight;
}