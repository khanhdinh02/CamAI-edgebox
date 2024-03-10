namespace CamAI.EdgeBox.Services.AI;

public static class AiProcessManager
{
    private static readonly List<AiProcessWrapper> RunningProcess = [];

    public static void Run(string processName, Uri uri, string path, IServiceProvider provider)
    {
        // if (RunningProcess.Exists(x => x.AiProcess.Name == processName))
        //     return;
        //
        // var aiProcess = new AiProcessWrapper(processName);
        // aiProcess.Run(uri, path);
        // RunningProcess.Add(aiProcess);
    }

    public static void Run(string processName, IServiceProvider provider)
    {
        // TODO [Duy]: validate edge box has shop before running AI
        if (RunningProcess.Exists(x => x.Name == processName))
            return;

        var aiProcess = new AiProcessWrapper(processName, provider);
        aiProcess.Run(new Uri("rtsp://admin:Admin123%40@localhost:554/Streaming/channels/101"), "");

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
