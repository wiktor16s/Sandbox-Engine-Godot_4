using System.Threading;

namespace SandboxEngine;

public class RenderThread
{
    public bool   IsBusy;
    public bool   ShouldRenderTexture;
    public int    ThreadId;
    public Thread WorkingThread;

    public RenderThread(int threadId)
    {
        ThreadId            = threadId;
        IsBusy              = true;
        ShouldRenderTexture = false;
        WorkingThread       = new Thread(_job);
    }

    public void StartTask()
    {
        IsBusy = true;
    }

    public void StopTask()
    {
        IsBusy = false;
    }

    private void _job(object obj)
    {
        ThreadId = (int)obj;
        var renderer = RenderManager.GetRendererByIndex(ThreadId);
        while (true)
        {
            if (IsBusy)
            {
                if (ShouldRenderTexture)
                    renderer.UpdateTexture();
                else
                    renderer.ProcessChunk(ThreadManager.ActualChunkIteration);

                StopTask();
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }
}