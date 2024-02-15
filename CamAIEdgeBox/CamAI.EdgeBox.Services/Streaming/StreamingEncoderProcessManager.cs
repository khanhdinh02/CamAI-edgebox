namespace CamAI.EdgeBox.Services.Streaming;

public static class StreamingEncoderProcessManager
{
    private static readonly List<StreamingEncoderProcessWrapper> RunningProcess = [];

    public static string RunEncoder(string processName, Uri uri, string path)
    {
        var process = RunningProcess.Find(x => x.Name == processName);
        if (process != null)
            return process.M3U8File;

        var ffmpegProcess = new StreamingEncoderProcessWrapper(processName, uri, path);
        RunningProcess.Add(ffmpegProcess);
        ffmpegProcess.Run();
        return ffmpegProcess.M3U8File;
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
