using FFMpegCore;
using Timer = System.Timers.Timer;

namespace CamAI.EdgeBox.Services.Streaming;

public class StreamingEncoderProcessWrapper
{
    public bool Running { get; private set; }
    public string Name { get; }
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly Timer timer;
    private readonly Uri inputUrl;
    private readonly string outputPath;
    public string M3U8File => Path.Combine(outputPath, $"{Name}.m3u8");

    public StreamingEncoderProcessWrapper(string name, Uri inputUrl, string outputPath)
    {
        cancellationTokenSource = new CancellationTokenSource();
        timer = CreateTimer();
        Name = name;
        this.inputUrl = inputUrl;
        this.outputPath = outputPath;
    }

    private Timer CreateTimer()
    {
        var newTimer = new Timer(TimeSpan.FromMinutes(3));
        newTimer.AutoReset = false;
        newTimer.Elapsed += (_, _) => StreamingEncoderProcessManager.Kill(Name);
        return newTimer;
    }

    public void ResetTimer()
    {
        timer.Stop();
        timer.Start();
    }

    public void Run()
    {
        FFMpegArguments
            .FromUrlInput(inputUrl)
            .OutputToFile(
                M3U8File,
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

    // public void Run()
    // {
    //     Running = true;
    //     FFMpegArguments
    //         .FromFileInput(
    //             "C:\\Users\\ngud\\Downloads\\ffmpeg-6.1.1-essentials_build\\bin\\big_buck_bunny_640x360.ts",
    //             true,
    //             args =>
    //             {
    //                 args.WithCustomArgument("-stream_loop -1");
    //             }
    //         )
    //         .OutputToFile(M3U8File)
    //         .CancellableThrough(cancellationTokenSource.Token)
    //         .ProcessAsynchronously();
    //     Thread.Sleep(5000);
    //     timer.Start();
    // }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        timer.Dispose();
        cancellationTokenSource.Dispose();
        CleanUpTsFile();
        Running = false;
    }

    private void CleanUpTsFile()
    {
        // delete ts file
        var tsFiles = Directory.EnumerateFiles(outputPath, "*.ts", SearchOption.TopDirectoryOnly);
        foreach (var file in tsFiles)
            File.Delete(file);
        // delete m3u8 file
        File.Delete(
            Directory.EnumerateFiles(outputPath, "*.m3u8", SearchOption.TopDirectoryOnly).First()
        );
    }
}
