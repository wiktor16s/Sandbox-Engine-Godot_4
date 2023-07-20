using System.Diagnostics;
using Godot;

namespace SandboxEngine.Utils;

public class FpsCounter
{
    public ulong     count;
    public int       everyXTimeCounter;
    public Stopwatch stopwatch = new();
    public long      sumTicks;


    public void Start()
    {
        stopwatch.Start();
    }

    public void Tick()
    {
        sumTicks += stopwatch.ElapsedTicks;
        count++;
        stopwatch.Restart();
    }

    public void GetFPS()
    {
        Tick();

        var fps = Stopwatch.Frequency / (double)sumTicks * count;
        GD.Print("FPS: " + fps);
    }

    public void GetFPSAndRestart()
    {
        GetFPS();
        count    = 0;
        sumTicks = 0;
    }

    public void GetFpsEveryXTime(int x)
    {
        Tick();
        everyXTimeCounter++;
        if (everyXTimeCounter % x == 0)
        {
            GetFPS();
            everyXTimeCounter = 0;
        }
    }
}