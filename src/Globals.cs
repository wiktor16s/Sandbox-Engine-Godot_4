namespace SandboxEngine;

public static class Globals
{
    public const  float Gravitation              = 1f;
    public const  int   AmountOfChunksInRenderer = 4; // Always 4
    public static int   MaxFps                   = 0;
    public static int   MapRendererWidth         = 128;
    public static int   MapRendererHeight        = MapRendererWidth;
    public static int   GridRendererWidth        = 9;
    public static int   GridRendererHeight       = 9;
    public static int   RendererScale            = 1;
    public static int   GridTotalRenderers       = GridRendererWidth * GridRendererHeight;
}