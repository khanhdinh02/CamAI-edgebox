using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class AiProcessWrapper(string name, IServiceProvider provider)
{
    public string Name => name;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private ClassifierProcessor? classifier;

    public void Run(Uri inputUrl, string outputPath)
    {
        var bus = provider.GetRequiredService<IPublishEndpoint>();
        var configuration = provider.GetRequiredService<IOptions<AiConfiguration>>();
        classifier = new ClassifierProcessor(configuration, bus);
#pragma warning disable 4014
        classifier.Start(cancellationTokenSource.Token);
        // TODO [Duy]: run AI with input and output
        // TODO: get running AI command str
        // TODO: create a process to run AI command
        // TODO: wait until AI is running
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        classifier?.Dispose();
    }
}
