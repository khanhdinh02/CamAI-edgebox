using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.AI;

public static class AiProcessManager
{
    private static readonly List<AiProcessWrapper> RunningProcess = [];

    public static void Run(string processName, Camera camera, IServiceProvider provider)
    {
        // TODO [Duy]: validate edge box has shop before running AI
        if (RunningProcess.Exists(x => x.Name == processName))
            return;

        var aiProcess = new AiProcessWrapper(processName, provider);
        aiProcess.Run(camera);

        RunningProcess.Add(aiProcess);
    }

    public static void Kill(string processName)
    {
        var process = RunningProcess.Find(x => x.Name == processName);
        if (process == null)
            return;

        RunningProcess.Remove(process);
        process.Kill();
    }
}
