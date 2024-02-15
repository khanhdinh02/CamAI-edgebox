namespace CamAI.EdgeBox.Services.Streaming;

public static class StreamingEncoderProcessManager
{
    private static readonly List<StreamingEncoderProcessWrapper> RunningProcess = [];

    public static void RunEncoder(string processName, Uri uri, string path)
    {
        if (RunningProcess.Exists(x => x.Name == processName))
            return;

        var ffmpegProcess = new StreamingEncoderProcessWrapper(processName);
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

    public static void UpdateTimer(string processName) =>
        RunningProcess.Find(x => x.Name == processName)?.ResetTimer();
}
