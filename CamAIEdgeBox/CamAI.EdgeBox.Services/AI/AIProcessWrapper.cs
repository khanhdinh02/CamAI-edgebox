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
        // TODO [Duy]: maybe use classifier for human count as well
        // var humanCountProcessor = new HumanCountProcessor(configuration, bus);
        var classifier = new ClassifierProcessor(configuration, bus);
#pragma warning disable 4014
        // humanCountProcessor.Start(cancellationTokenSource.Token);
        classifier.Start(cancellationTokenSource.Token);
        // TODO [Duy]: run AI with input and output
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
