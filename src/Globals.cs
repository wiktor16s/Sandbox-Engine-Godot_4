namespace SandboxEngine;

public static class Globals
{
    public const float Gravitation = 9.81f;
    public const int NeverFreezeValue = int.MaxValue;
    public const int NeverBurnValue = int.MinValue;
    public static uint UpdateTickCounter = 0;
    public static bool tickOscillator = false;
}