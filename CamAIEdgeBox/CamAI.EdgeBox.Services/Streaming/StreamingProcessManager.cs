using CamAI.EdgeBox.Models;
using Serilog;

namespace CamAI.EdgeBox.Services.Streaming;

public static class StreamingProcessManager
{
    private static readonly List<StreamingProcess> RunningProcess = [];

    public static void RunEncoder(Camera camera, Uri httpRelayUri)
    {
        var p = RunningProcess.Find(x => x.Name == camera.Id.ToString("N"));
        if (p != null)
            return;

        Log.Information("Start streaming process for camera {CameraId}", camera.Id);
        var ffmpegProcess = new StreamingProcess(camera, httpRelayUri);
        RunningProcess.Add(ffmpegProcess);
        ffmpegProcess.Run();
    }

    public static void Kill(Camera camera)
    {
        Log.Information("Kill streaming process for camera {CameraId}", camera.Id);
        var process = RunningProcess.Find(x => x.Name == camera.Id.ToString("N"));
        if (process == null)
            return;

        Log.Information("Found streaming process for camera {CameraId}, killing it", camera.Id);
        process.Dispose();
        RunningProcess.Remove(process);
    }
}
