namespace CamAI.EdgeBox.Services.AI;

public static class AIProcessManager
{
    // TODO: include a result watcher for each process
    private static readonly List<AIProcessWrapper> RunningProcess = [];

    public static void RunEncoder(string processName, Uri uri, string path)
    {
        if (RunningProcess.Exists(x => x.Name == processName))
            return;

        var ffmpegProcess = new AIProcessWrapper(processName);
        ffmpegProcess.Run(uri, path);
        RunningProcess.Add(ffmpegProcess);
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
