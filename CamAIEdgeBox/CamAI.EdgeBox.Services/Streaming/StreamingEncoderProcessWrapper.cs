using FFMpegCore;
using Timer = System.Timers.Timer;

namespace CamAI.EdgeBox.Services.Streaming;

public class StreamingEncoderProcessWrapper
{
    public string Name { get; }
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly Timer timer;

    public StreamingEncoderProcessWrapper(string name)
    {
        cancellationTokenSource = new CancellationTokenSource();
        timer = CreateTimer();
        Name = name;
    }

    private Timer CreateTimer()
    {
        var newTimer = new Timer(TimeSpan.FromMinutes(3));
        newTimer.AutoReset = false;
        newTimer.Elapsed += (_, _) => Kill();
        return newTimer;
    }

    public void ResetTimer()
    {
        timer.Stop();
        timer.Start();
    }

    public void Run(Uri inputUrl, string outputPath)
    {
        FFMpegArguments
            .FromUrlInput(inputUrl)
            .OutputToFile(
                outputPath,
                true,
                args =>
                {
                    args.WithCustomArgument(
                        "-fflags flush_packets -max_delay 5 -flags -global_header -hls_flags delete_segments -hls_time 3 -hls_list_size 20 -y -an -vcodec h264"
                    );
                }
            )
            .CancellableThrough(cancellationTokenSource.Token)
            .ProcessAsynchronously();
        Thread.Sleep(5000);
        timer.Start();
    }

    public void Run()
    {
        FFMpegArguments
            .FromFileInput(
                "C:\\Users\\ngud\\Downloads\\ffmpeg-6.1.1-essentials_build\\bin\\big_buck_bunny_640x360.ts",
                true,
                args =>
                {
                    args.WithCustomArgument("-stream_loop -1");
                }
            )
            .OutputToFile(
                "C:\\Users\\ngud\\Downloads\\ffmpeg-6.1.1-essentials_build\\bin\\example.m3u8"
            )
            .CancellableThrough(cancellationTokenSource.Token)
            .ProcessAsynchronously();
        Thread.Sleep(5000);
        timer.Start();
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        timer.Dispose();
        cancellationTokenSource.Dispose();
    }
}
