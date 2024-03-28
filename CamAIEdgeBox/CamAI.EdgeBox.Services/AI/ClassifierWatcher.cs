﻿using System.Text.Json;
using CamAI.EdgeBox.Services.Utils;
using Serilog;

namespace CamAI.EdgeBox.Services.AI;

public class ClassifierWatcher : IDisposable
{
    private bool disposed;

    private static readonly JsonSerializerOptions Options =
        new() { PropertyNameCaseInsensitive = true };

    private readonly FileSystemWatcher fileWatcher;
    private readonly string outputFile;
    private readonly string outputSeparator;
    public event ClassifierNotify? Notifier;
    private readonly string watchFile;

    public ClassifierWatcher(string watchDirectory, string outputFile, string outputSeparator)
    {
        Log.Information("Creating watcher");
        this.outputSeparator = outputSeparator;
        this.outputFile = outputFile;
        fileWatcher = new FileSystemWatcher(watchDirectory);
        watchFile = Path.Combine(watchDirectory, outputFile);
        ConfigureFilters();
    }

    private void ConfigureFilters()
    {
        fileWatcher.Changed += HandleFileChange;
        fileWatcher.Filter = outputFile;
        fileWatcher.EnableRaisingEvents = true;
    }

    private void HandleFileChange(object sender, FileSystemEventArgs e)
    {
        var lastLine = Retry.Do(
            () => File.ReadLines(watchFile).Last().Split(outputSeparator),
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
