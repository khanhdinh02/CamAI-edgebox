using CamAI.EdgeBox.Services.Utils;
using MassTransit;

namespace CamAI.EdgeBox.Services.AI.Uniform;

public class UniformProcessor : IDisposable
{
    private bool disposed;
    private readonly UniformConfiguration uniform;
    private readonly RtspExtension rtsp;
    private readonly IPublishEndpoint bus;

    public UniformProcessor(
        ClassifierWatcher watcher,
        RtspExtension rtsp,
        UniformConfiguration uniformConfiguration,
        IPublishEndpoint bus
    )
    {
        uniform = uniformConfiguration;
        this.bus = bus;
        this.rtsp = rtsp;
        watcher.Notifier += ReceiveData;
    }

    public async Task Start(CancellationToken cancellationToken) { }

    // TODO: within last 300s, if there is less than 100 detected uniform than it is incident

    private void ReceiveData(int time, List<ClassifierOutputModel> output)
    {
        // TODO: filter and add data
        // if (output.Count > 0)
        //     classifierOutputs.Add(output);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool _)
    {
        if (disposed)
            return;

        disposed = true;
    }
}
