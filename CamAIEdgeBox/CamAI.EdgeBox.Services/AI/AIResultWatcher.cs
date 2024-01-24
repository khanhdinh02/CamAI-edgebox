using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class AIResultWatcher(IOptions<AIConfiguration> aiConfiguration)
{
    private readonly FileSystemWatcher fileWatcher = new FileSystemWatcher(
        aiConfiguration.Value.OutputDirectory
    );
    private readonly AIConfiguration aiConfiguration = aiConfiguration.Value;

    private void ConfigureFilters()
    {
        fileWatcher.Filter = "*.txt";
    }
}
