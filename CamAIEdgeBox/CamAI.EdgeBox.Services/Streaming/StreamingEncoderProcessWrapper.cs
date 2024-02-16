using FFMpegCore;

namespace CamAI.EdgeBox.Services.Streaming;

public class StreamingEncoderProcessWrapper(string name, Uri inputUrl, string outputPath)
    : IDisposable
{
    private bool disposed;
    public bool Running { get; private set; }
    public string Name { get; } = name;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly Uri inputUrl = inputUrl;
    public string M3U8File => Path.Combine(outputPath, $"{Name}.m3u8");

    // public void Run()
    // {
    //     Running = true;
    //     FFMpegArguments
    //         .FromUrlInput(inputUrl)
    //         .OutputToFile(
    //             M3U8File,
    //             true,
    //             args =>
    //             {
    //                 args.WithCustomArgument(
    //                     "-fflags flush_packets -max_delay 5 -flags -global_header -hls_flags delete_segments -hls_time 3 -hls_list_size 20 -y -an -vcodec h264"
    //                 );
    //             }
    //         )
    //         .CancellableThrough(cancellationTokenSource.Token)
    //         .ProcessAsynchronously();
    // }

    public void Run()
    {
        Running = true;
        FFMpegArguments
            .FromFileInput(
                "C:\\Users\\ngud\\Downloads\\ffmpeg-6.1.1-essentials_build\\bin\\big_buck_bunny_640x360.ts",
                true,
                args =>
                {
                    args.WithCustomArgument("-stream_loop -1");
                }
            )
            .OutputToFile(M3U8File)
            .CancellableThrough(cancellationTokenSource.Token)
            .ProcessAsynchronously();
        WaitForFfMpegProcess();
    }

    private void WaitForFfMpegProcess()
    {
        while (!Directory.EnumerateFiles(outputPath, "*.ts", SearchOption.TopDirectoryOnly).Any())
            Thread.Sleep(1000);
    }

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
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            CleanUpTsFile();
            Running = false;
        }
        disposed = true;
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
