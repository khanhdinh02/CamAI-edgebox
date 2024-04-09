using System.Diagnostics;
using System.Text;
using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.AI.Uniform;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace CamAI.EdgeBox.Services.AI;

public class AiProcessWrapper(Camera camera, IServiceProvider provider)
{
    public string Name => camera.ToName();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private Process? aiProcess;
    private AiProcessUtil? aiProcessUtil;
    private ClassifierWatcher? watcher;
    private HumanCountProcessor? humanCount;
    private InteractionProcessor? interaction;
    private PhoneProcessor? phone;
    private UniformProcessor? uniform;

    public bool IsRunning
    {
        get
        {
            if (aiProcess == null)
                return false;
            try
            {
                Process.GetProcessById(aiProcess.Id);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public void Run()
    {
        Log.Information("Running AI Process");
        var publishBus = provider.GetRequiredService<IPublishEndpoint>();
        var configuration = provider.GetRequiredService<IOptions<AiConfiguration>>().Value;

        var recordOutputPath = camera.Path.Split("/")[^1];
        var aiOutputPath = Path.Combine(configuration.BaseDirectory, "records", recordOutputPath);
        CleanDirectory(aiOutputPath);

        aiProcess = CreateNewAiProcess(configuration, camera.GetUri());
        aiProcess.Start();
        WaitForAiOutput(aiOutputPath);
        aiProcessUtil = new AiProcessUtil(configuration, camera, aiProcess!);

        watcher = new ClassifierWatcher(
            aiOutputPath,
            configuration.OutputFile,
            configuration.OutputSeparator
        );

        humanCount = new HumanCountProcessor(watcher, configuration.HumanCount, publishBus);
        phone = new PhoneProcessor(watcher, aiProcessUtil, configuration.Phone, publishBus);
        uniform = new UniformProcessor(watcher, aiProcessUtil, configuration.Uniform, publishBus);
        interaction = new InteractionProcessor(
            watcher,
            aiProcessUtil,
            configuration.Interaction,
            publishBus
        );

#pragma warning disable 4014
        Task.Run(() => humanCount.Start(cancellationTokenSource.Token));
        Task.Run(() => phone.Start(cancellationTokenSource.Token));
        Task.Run(() => uniform.Start(cancellationTokenSource.Token));
        Task.Run(() => interaction.Start(cancellationTokenSource.Token));
    }

    private static Process CreateNewAiProcess(AiConfiguration configuration, Uri cameraUri)
    {
        Log.Information("Create new AI Process");
        var process = new Process();
        process.StartInfo.WorkingDirectory = configuration.BaseDirectory;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.FileName = configuration.ProcessFileName;

        var argumentsBuilder = new StringBuilder(configuration.ProcessArgument);
        argumentsBuilder.Replace("{BaseDirectory}", configuration.BaseDirectory);
        argumentsBuilder.Replace("{CameraUri}", cameraUri.ToString());
        process.StartInfo.Arguments = argumentsBuilder.ToString();

        Log.Information(
            "Running process command {FileName} {Arguments}",
            process.StartInfo.FileName,
            process.StartInfo.Arguments
        );
        return process;
    }

    private static void WaitForAiOutput(string aiOutputPath)
    {
        Log.Information("Waiting for AI Output");
        while (
            !Directory.Exists(aiOutputPath)
            || !Directory.EnumerateFiles(aiOutputPath, "*", SearchOption.TopDirectoryOnly).Any()
        )
            Thread.Sleep(1000);
        Log.Information("AI output detected");
    }

    private static void CleanDirectory(string path)
    {
        Log.Information("Clean AI output directory");
        if (!Directory.Exists(path))
            return;
        var files = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
        foreach (var file in files)
            File.Delete(file);
    }

    public void Kill()
    {
        Log.Information("Kill process {Name}", Name);
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        aiProcess?.Kill();
        humanCount?.Dispose();
        phone?.Dispose();
        uniform?.Dispose();
        watcher?.Dispose();
        aiProcessUtil?.CleanUpEvidence();
    }

    public class AiProcessUtil(AiConfiguration configuration, Camera camera, Process aiProcess)
    {
        public Guid CameraId => camera.Id;

        public string CaptureFrame(string outputFileName)
        {
            outputFileName += ".png";
            var datetime = DateTime.Now;
            var dateTimeDir = Path.Combine(
                datetime.Year.ToString(),
                datetime.Month.ToString(),
                datetime.Day.ToString()
            );
            var outputPath = Path.Combine(configuration.EvidenceOutputDir, dateTimeDir);

            Directory.CreateDirectory(outputPath);
            var file = Path.Combine(outputPath, outputFileName).Replace('\\', '/');
            Log.Information("Capture evidence to path {Path}", file);
            aiProcess.StandardInput.WriteLine($"capture {file}");
            return file;
        }

        public void CleanUpEvidence() => IOUtil.DeleteDirectory(configuration.EvidenceOutputDir);
    }
}
