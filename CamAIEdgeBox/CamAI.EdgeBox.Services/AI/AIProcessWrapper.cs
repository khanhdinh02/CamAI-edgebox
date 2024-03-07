using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class AiProcessWrapper(string name, IServiceProvider provider)
{
    public string Name => name;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private HumanCountProcessor? humanCount;

    public void Run(Uri inputUrl, string outputPath)
    {
        var bus = provider.GetRequiredService<IPublishEndpoint>();
        var configuration = provider.GetRequiredService<IOptions<AiConfiguration>>();
        humanCount = new HumanCountProcessor(configuration, bus);
#pragma warning disable 4014
        humanCount.Start(cancellationTokenSource.Token);
        // TODO [Duy]: run AI with input and output
        // TODO: get running AI command str
        // TODO: create a process to run AI command
        // TODO: wait until AI is running
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        humanCount?.Dispose();
    }
}
