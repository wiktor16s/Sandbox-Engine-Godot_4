using System;
using System.Diagnostics;
using Godot;

namespace SandboxEngine;

public static class ThreadManager
{
    public static int            ActualChunkIteration;
    public static RenderThread[] RenderThreads = new RenderThread[Globals.GridTotalRenderers];
    public static Stopwatch      Stopwatch     = new();
    public static long           test;

    public static void InitRenderThreads()
    {
        for (var i = 0; i < Globals.GridTotalRenderers; i++)
        {
            RenderThreads[i] = new RenderThread(i);
            RenderThreads[i].WorkingThread.Start(i);
            Stopwatch.Start();
        }
    }

    public static void ChunksIteration()
    {
        test += 1;

        for (var i = 0; i < Globals.GridTotalRenderers; i++)
            if (RenderThreads[i].IsBusy)
                return;
        
        if (ActualChunkIteration > Globals.GridTotalRenderers - 1)
        {
            ActualChunkIteration = -1;
        }
        else
        {
            ActualChunkIteration++;
        }

        if (ActualChunkIteration == 0)
        {
            for (var i = 0; i < Globals.GridTotalRenderers; i++)
            {
                RenderThreads[i].ShouldRenderTexture = false;
                RenderThreads[i].StartTask();
            }
        }
        else if (ActualChunkIteration >= 0 && ActualChunkIteration <= Globals.AmountOfChunksInRenderer - 1)
        {
            for (var i = 0; i < Globals.GridTotalRenderers; i++)
                RenderThreads[i].StartTask();
        }

        else if (ActualChunkIteration == -1)
        {
            for (var i = 0; i < Globals.GridTotalRenderers; i++)
            {
                RenderThreads[i].ShouldRenderTexture = true;
                RenderThreads[i].StartTask();
            }

            if (test % 1000 == 0)
                GD.Print(new TimeSpan(0, 0, 1) / Stopwatch.Elapsed);
            Stopwatch.Restart();
        }
    }
}

/*
 * if (test % 2000 == 0)
                GD.Print(new TimeSpan(0, 0, 1) / Stopwatch.Elapsed);
            Stopwatch.Restart();
*/