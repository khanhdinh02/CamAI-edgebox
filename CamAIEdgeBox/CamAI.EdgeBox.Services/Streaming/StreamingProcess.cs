using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.Utils;
using FFMpegCore;

namespace CamAI.EdgeBox.Services.Streaming;

public class StreamingProcess(Camera camera, Uri httpRelayUri) : IDisposable
{
    private bool disposed;
    public bool Running { get; private set; }
    public string Name => camera.Id.ToString("N");
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public void Run()
    {
        Running = true;
        var result = FFMpegArguments
            .FromUrlInput(
                camera.GetUri(),
                options =>
                {
                    options.WithCustomArgument("-hwaccel cuda");
                }
            )
            .OutputToUrl(
                httpRelayUri,
                options =>
                {
                    options.WithCustomArgument(
                        "-f mpegts -codec:v mpeg1video -s 640x480 -b:v 1000k -bf 0"
                    );
                }
            )
            .CancellableThrough(cancellationTokenSource.Token)
            .ProcessAsynchronously();
        result.ContinueWith(_ => StreamingProcessManager.Kill(camera));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;
        if (disposing)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            Running = false;
        }
        disposed = true;
    }
}
