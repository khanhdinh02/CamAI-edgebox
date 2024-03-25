using System.Diagnostics;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.AI.Uniform;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public class AiProcessWrapper(Camera camera, IServiceProvider provider)
{
    public string Name => camera.ToName();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private Process? aiProcess;
    private ClassifierWatcher? watcher;
    private HumanCountProcessor? humanCount;
    private InteractionProcessor? interaction;
    private PhoneProcessor? detection;
    private UniformProcessor? uniform;

    public void Run()
    {
        var publishBus = provider.GetRequiredService<IPublishEndpoint>();
        var configuration = provider.GetRequiredService<IOptions<AiConfiguration>>().Value;
        var rtsp = new RtspExtension(camera, configuration.EvidenceOutputDir);

        var recordOutputPath = camera.Path.Split("/")[^1];
        var aiOutputPath = Path.Combine(configuration.BaseDirectory, "records", recordOutputPath);
        CleanDirectory(aiOutputPath);

        // TODO: get base project directory
        aiProcess = CreateNewAiProcess(
            "/mnt/c/project/camai/Human-Activity-Monitor",
            camera.GetUri()
        );
        aiProcess.Start();
        WaitForAiOutput(aiOutputPath);

        watcher = new ClassifierWatcher(
            aiOutputPath,
            configuration.OutputFile,
            configuration.OutputSeparator
        );

        humanCount = new HumanCountProcessor(watcher, configuration.HumanCount, publishBus);
        detection = new PhoneProcessor(watcher, rtsp, configuration.Phone, publishBus);
        uniform = new UniformProcessor(watcher, rtsp, configuration.Uniform, publishBus);
        interaction = new InteractionProcessor(watcher, configuration.Interaction, publishBus);

#pragma warning disable 4014
        Task.Run(() => humanCount.Start(cancellationTokenSource.Token));
        Task.Run(() => detection.Start(cancellationTokenSource.Token));
        Task.Run(() => uniform.Start(cancellationTokenSource.Token));
        Task.Run(() => interaction.Start(cancellationTokenSource.Token));
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
