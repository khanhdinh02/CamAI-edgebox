using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.Utils;
using Serilog;

namespace CamAI.EdgeBox.Services.AI;

public static class AiProcessManager
{
    private static readonly List<AiProcessWrapper> RunningProcess = [];

    public static void Run(Camera camera, IServiceProvider provider)
    {
        Log.Information("Receive request to run AI process for camera id {CameraId}", camera.Id);
        if (RunningProcess.Exists(x => x.Name == camera.ToName()))
        {
            Log.Information(
                "There is already running AI process for camera id {CameraId}",
                camera.Id
            );
            return;
        }

        Log.Information("Create new AI process for camera id {CameraId}", camera.Id);
        var aiProcess = new AiProcessWrapper(camera, provider);
        aiProcess.Run();

        RunningProcess.Add(aiProcess);
    }

    public static void Kill(Camera camera)
    {
        var process = RunningProcess.Find(x => x.Name == camera.ToName());
        if (process == null)
            return;

        RunningProcess.Remove(process);
        process.Kill();
    }

    public static void KillAll()
    {
        Log.Information("Killing all running AI process");
        foreach (var process in RunningProcess)
        {
            RunningProcess.Remove(process);
            process.Kill();
        }
    }

    public static int NumOfProcess => RunningProcess.Count;
    public static int NumOfRunningProcess => RunningProcess.Count(x => x.IsRunning);
}
