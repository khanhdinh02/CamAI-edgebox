using System.Diagnostics;
using CamAI.EdgeBox.Models;
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
    private Process? aiProcess;
    private HumanCountProcessor? humanCount;
    private ClassifierWatcher? watcher;
    private DetectionProcessor? detection;
    private UniformProcessor? uniform;

    public void Run(Camera camera)
    {
        var publishBus = provider.GetRequiredService<IPublishEndpoint>();
        var configuration = provider.GetRequiredService<IOptions<AiConfiguration>>().Value;
        var cameraUri = camera.GetUri();
        var rtsp = new RtspExtension(cameraUri, configuration.EvidenceOutputDir);

        // TODO: get directory from cameraUri
        var aiOutputPath = Path.Combine(configuration.BaseDirectory, "records/101");
        CleanDirectory(aiOutputPath);

        aiProcess = CreateNewAiProcess("/mnt/c/project/camai/Human-Activity-Monitor", cameraUri);
        aiProcess.Start();
        WaitForAiOutput(aiOutputPath);

        watcher = new ClassifierWatcher(
            aiOutputPath,
            configuration.OutputFile,
            configuration.OutputSeparator
        );

        humanCount = new HumanCountProcessor(watcher, configuration.HumanCount, publishBus);
        detection = new DetectionProcessor(watcher, rtsp, configuration.Detection, publishBus);
        uniform = new UniformProcessor(watcher, rtsp, configuration.Uniform, publishBus);

#pragma warning disable 4014
        Task.Run(() => humanCount.Start(cancellationTokenSource.Token));
        Task.Run(() => detection.Start(cancellationTokenSource.Token));
        Task.Run(() => uniform.Start(cancellationTokenSource.Token));
    }

    private static Process CreateNewAiProcess(string aiBaseDir, Uri cameraUri)
    {
        var process = new Process();
        process.StartInfo.FileName = "wsl";
        // TODO: disable show video
        process.StartInfo.Arguments =
            $"cd {aiBaseDir} && /bin/python3 {aiBaseDir}/src/run.py video.path={cameraUri} video.speed=1";
        return process;
    }

    private static void WaitForAiOutput(string aiOutputPath)
    {
        while (!Directory.EnumerateFiles(aiOutputPath, "*", SearchOption.TopDirectoryOnly).Any())
            Thread.Sleep(1000);
    }

    private static void CleanDirectory(string path)
    {
        var files = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
            File.Delete(file);
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        aiProcess?.Kill();
        humanCount?.Dispose();
        detection?.Dispose();
        uniform?.Dispose();
        watcher?.Dispose();
    }
}
