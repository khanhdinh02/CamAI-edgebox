using CamAI.EdgeBox.Services.AI.Uniform;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class AiProcessWrapper(string name, IServiceProvider provider)
{
    public string Name => name;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private HumanCountProcessor? humanCount;
    private ClassifierWatcher? watcher;
    private DetectionProcessor? detection;
    private UniformProcessor? uniform;

    public void Run(Uri inputUrl, string outputPath)
    {
        // TODO [Duy]: run AI with input and output
        // TODO: get running AI command str
        // TODO: create a process to run AI command
        // TODO: wait until AI is running

        var publishBus = provider.GetRequiredService<IPublishEndpoint>();
        var configuration = provider.GetRequiredService<IOptions<AiConfiguration>>();
        var rtsp = new RtspExtension(inputUrl, configuration.Value.EvidenceOutputDir);

        watcher = new ClassifierWatcher(configuration);

        humanCount = new HumanCountProcessor(watcher, configuration, publishBus);
        detection = new DetectionProcessor(watcher, rtsp, configuration, publishBus);
        uniform = new UniformProcessor(watcher, rtsp, configuration, publishBus);

#pragma warning disable 4014
        Task.Run(() => humanCount.Start(cancellationTokenSource.Token));
        Task.Run(() => detection.Start(cancellationTokenSource.Token));
        Task.Run(() => uniform.Start(cancellationTokenSource.Token));
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        humanCount?.Dispose();
        detection?.Dispose();
        uniform?.Dispose();
        watcher?.Dispose();
    }
}
