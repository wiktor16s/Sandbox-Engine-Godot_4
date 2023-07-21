using SandboxEngine.Utils;

namespace SandboxEngine;

public static class ThreadManager
{
    public static           int            ActualChunkIteration;
    public static           RenderThread[] RenderThreads = new RenderThread[Globals.GridTotalRenderers];
    private static readonly FpsCounter     _fpsCounter   = new();

    public static void InitRenderThreads()
    {
        for (var i = 0; i < Globals.GridTotalRenderers; i++)
        {
            RenderThreads[i] = new RenderThread(i);
            RenderThreads[i].WorkingThread.Start(i);
            _fpsCounter.Start();
        }
    }

    public static void ChunksIteration()
    {
        foreach (var thread in RenderThreads)
            if (thread.IsBusy)
                return;

        if (ActualChunkIteration > Globals.GridTotalRenderers - 1)
            ActualChunkIteration = -1;
        else
            ActualChunkIteration++;


        if (ActualChunkIteration == 0)
        {
            _fpsCounter.GetFpsEveryXTime(10);
            foreach (var thread in RenderThreads)
            {
                thread.ShouldRenderTexture = false;
                thread.Signal.Set();
            }
        }

        else if (ActualChunkIteration >= 0 && ActualChunkIteration <= Globals.AmountOfChunksInRenderer - 1)
        {
            foreach (var thread in RenderThreads)
                thread.Signal.Set();
        }

        else if (ActualChunkIteration == -1)
        {
            foreach (var rendererX in RenderManager.Renderers)
            {
                foreach (var rendererY in rendererX)
                {
                    rendererY.UpdateTexture();
                }
            }
        }
    }
}