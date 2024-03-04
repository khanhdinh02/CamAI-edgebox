using System.Text.Json;
using CamAI.EdgeBox.Services.Utils;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class ClassifierWatcher : IDisposable
{
    private bool disposed;

    private static readonly JsonSerializerOptions Options =
        new() { PropertyNameCaseInsensitive = true };

    private readonly FileSystemWatcher fileWatcher;
    private readonly AiConfiguration aiConfiguration;
    public event ClassifierNotify? Notifier;
    private readonly string watchFile;

    public ClassifierWatcher(IOptions<AiConfiguration> aiConfiguration)
    {
        fileWatcher = new FileSystemWatcher(aiConfiguration.Value.OutputDirectory);
        this.aiConfiguration = aiConfiguration.Value;
        watchFile = Path.Combine(
            this.aiConfiguration.OutputDirectory,
            this.aiConfiguration.OutputFile
        );
        ConfigureFilters();
    }

    private void ConfigureFilters()
    {
        fileWatcher.Changed += HandleFileChange;
        fileWatcher.Filter = aiConfiguration.OutputFile;
        fileWatcher.EnableRaisingEvents = true;
    }

    private void HandleFileChange(object sender, FileSystemEventArgs e)
    {
        var lastLine = Retry.Do(
            () => File.ReadLines(watchFile).Last().Split(aiConfiguration.OutputSeparator),
            TimeSpan.FromSeconds(1)
        );
        if (!int.TryParse(lastLine[0], out var time))
            return;

        var result = JsonSerializer.Deserialize<List<ClassifierOutputModel>>(
            lastLine[1].Replace('\'', '"'),
            Options
        );
        Notifier?.Invoke(time, result!);
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
            fileWatcher.Dispose();
        disposed = true;
    }
}

public class ClassifierOutputModel
{
    public int Id { get; set; }
    public ClassifierData Data { get; set; } = null!;
}

public class ClassifierData
{
    public string Label { get; set; } = null!;
    public double Score { get; set; }
}

public delegate void ClassifierNotify(int time, List<ClassifierOutputModel> output);
