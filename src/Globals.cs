namespace SandboxEngine;

public static class Globals
{
    public const  float Gravitation              = 1f;
    public static int   MaxFps                   = 0;
    public static int   MapRendererWidth         = 128;
    public static int   MapRendererHeight        = MapRendererWidth;
    public static int   GridRendererWidth        = 2;
    public static int   GridRendererHeight       = 2;
    public static int   RendererScale            = 2;
    public static int   AmountOfChunksInRenderer = 4;
    public static int   GridTotalRenderers       = GridRendererWidth * GridRendererHeight;
}