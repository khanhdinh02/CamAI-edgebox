using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.AI;

public class AIProcessWrapper(string name)
{
    public string Name => name;
    private readonly CancellationTokenSource cancellationTokenSource = new();

    public void Run(Uri inputUrl, string outputPath)
    {
        // TODO: run AI with input and output
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
