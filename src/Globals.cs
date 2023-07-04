namespace SandboxEngine;

public static class Globals
{
    public const  float Gravitation        = 1f;
    public const  int   NeverFreezeValue   = int.MaxValue;
    public const  int   NeverBurnValue     = int.MinValue;
    public static int   MaxFps             = 60;
    public static bool  TickOscillator     = false;
    public static int   MapChunkWidth      = 128;
    public static int   MapChunkHeight     = 128;
    public static int   GridWidth          = 2;
    public static int   GridHeight         = 2;
    public static int   RendererScale      = 4;
    public static int   GridTotalRenderers = GridWidth * GridHeight;
}