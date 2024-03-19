using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Services.Utils;

namespace CamAI.EdgeBox.Services.AI;

public static class AiProcessManager
{
    private static readonly List<AiProcessWrapper> RunningProcess = [];

    public static void Run(Camera camera, IServiceProvider provider)
    {
        if (RunningProcess.Exists(x => x.Name == camera.ToName()))
            return;

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
        foreach (var process in RunningProcess)
        {
            RunningProcess.Remove(process);
            process.Kill();
        }
    }
}
