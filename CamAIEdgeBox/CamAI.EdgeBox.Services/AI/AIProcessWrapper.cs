using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class AiProcessWrapper(string name, IServiceProvider provider)
{
    public string Name => name;
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public void Run(Uri inputUrl, string outputPath)
    {
        var bus = provider.GetRequiredService<IPublishEndpoint>();
        var configuration = provider.GetRequiredService<IOptions<AiConfiguration>>();
        var humanCountProcessor = new HumanCountProcessor(configuration, bus);
#pragma warning disable 4014
        humanCountProcessor.Start(cancellationTokenSource.Token);
        // TODO: run AI with input and output
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
