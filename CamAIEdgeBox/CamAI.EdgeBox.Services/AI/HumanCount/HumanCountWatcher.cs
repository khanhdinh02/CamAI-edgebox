using CamAI.EdgeBox.Services.Utils;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class HumanCountWatcher
{
    private readonly FileSystemWatcher fileWatcher;
    private readonly AiConfiguration aiConfiguration;
    public event HumanCountNotify Notifier;
    private string watchFile;

    public HumanCountWatcher(IOptions<AiConfiguration> aiConfiguration)
    {
        fileWatcher = new FileSystemWatcher(aiConfiguration.Value.OutputDirectory);
        // TODO: add dispose for filewatcher
        this.aiConfiguration = aiConfiguration.Value;
        watchFile = Path.Combine(
            this.aiConfiguration.OutputDirectory,
            this.aiConfiguration.HumanCount.Output
        );
        ConfigureFilters();
    }

    private void ConfigureFilters()
    {
        fileWatcher.Changed += HandleFileChange;
        fileWatcher.Filter = aiConfiguration.HumanCount.Output;
        fileWatcher.EnableRaisingEvents = true;
    }

    private void HandleFileChange(object sender, FileSystemEventArgs e)
    {
        var lastLine = Retry.Do(
            () => File.ReadLines(watchFile).Last().Split(","),
            TimeSpan.FromSeconds(1)
        );
        if (!int.TryParse(lastLine[0], out var time))
            return;
        var count = int.Parse(lastLine[1]);
        Notifier.Invoke(time, count);
    }
}

public delegate void HumanCountNotify(int time, int count);
