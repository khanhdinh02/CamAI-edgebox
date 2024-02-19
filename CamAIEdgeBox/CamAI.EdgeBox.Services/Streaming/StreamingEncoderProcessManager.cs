using Timer = System.Timers.Timer;

namespace CamAI.EdgeBox.Services.Streaming;

public static class StreamingEncoderProcessManager
{
    private static readonly List<ProcessWithTimer> RunningProcess = [];

    public static StreamingEncoderProcessManagerOption Option { get; set; } =
        new() { TimerInterval = 10 };

    public static string RunEncoder(string processName, Uri uri, string path)
    {
        var p = RunningProcess.Find(x => x.Process.Name == processName);
        if (p != null)
            return p.Process.M3U8File;

        var ffmpegProcess = new StreamingEncoderProcessWrapper(processName, uri, path);
        var timer = CreateTimer(processName);
        RunningProcess.Add(new ProcessWithTimer(ffmpegProcess, timer));
        ffmpegProcess.Run();
        return ffmpegProcess.M3U8File;
    }

    public static void Kill(string processName)
    {
        var process = RunningProcess.Find(x => x.Process.Name == processName);
        if (process == null)
            return;

        process.Dispose();
        RunningProcess.Remove(process);
    }

    public static void UpdateTimer(string processName) =>
        RunningProcess.Find(x => x.Process.Name == processName)?.Timer.Reset();

    private static Timer CreateTimer(string processName)
    {
        var newTimer = new Timer(TimeSpan.FromSeconds(Option.TimerInterval));
        newTimer.AutoReset = false;
        newTimer.Elapsed += (_, _) => Kill(processName);
        return newTimer;
    }

    private static void Reset(this Timer timer)
    {
        timer.Stop();
        timer.Start();
    }
}

public record ProcessWithTimer(StreamingEncoderProcessWrapper Process, Timer Timer) : IDisposable
{
    private bool disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;
        if (disposing)
        {
            Process.Dispose();
            Timer.Dispose();
        }
        disposed = true;
    }
}
