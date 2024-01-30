using FFMpegCore;
using FFMpegCore.Enums;

namespace CamAI.EdgeBox.Services.Streaming;

public class StreamingEncoderProcessWrapper(string name)
{
    public string Name => name;
    private readonly CancellationTokenSource cancellationTokenSource = new();

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
    }

    public void Kill()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
