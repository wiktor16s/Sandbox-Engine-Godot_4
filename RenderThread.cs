using System.Threading;

namespace SandboxEngine;

public class RenderThread
{
    public bool           IsBusy = true;
    public bool           ShouldRenderTexture;
    public AutoResetEvent Signal = new(false);
    public int            ThreadId;
    public Thread         WorkingThread;

    public RenderThread(int threadId)
    {
        ThreadId            = threadId;
        ShouldRenderTexture = false;
        WorkingThread       = new Thread(_job);
    }

    private void _job(object obj)
    {
        ThreadId = (int)obj;
        var renderer = RenderManager.GetRendererByIndex(ThreadId);
        while (true)
        {
            renderer.ProcessChunk(ThreadManager.ActualChunkIteration);
            IsBusy = false;
            Signal.WaitOne();
            IsBusy = true;
        }
    }
}