using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services.AI;

public static class AiProcessManager
{
    private static readonly List<AiProcessWrapper> RunningProcess = [];

    public static void Run(string processName, Uri uri, string path, IServiceProvider provider)
    {
        // TODO: get processor Event and add to watcher
        // if (RunningProcess.Exists(x => x.AiProcess.Name == processName))
        //     return;
        //
        // var aiProcess = new AiProcessWrapper(processName);
        // aiProcess.Run(uri, path);
        // RunningProcess.Add(aiProcess);
        // var humanCountProcessor = new HumanCountProcessor();
    }

    public static void Run(string processName, IServiceProvider provider)
    {
        // TODO: get processor Event and add to watcher
        if (RunningProcess.Exists(x => x.Name == processName))
            return;

        var aiProcess = new AiProcessWrapper(processName, provider);
        aiProcess.Run(new Uri("http://localhost/"), "");

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
