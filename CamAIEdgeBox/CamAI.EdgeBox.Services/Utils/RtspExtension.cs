using FFMpegCore;

namespace CamAI.EdgeBox.Services.Utils;

public class RtspExtension(Uri uri, string baseDir)
{
    public string CaptureFrame(string outputFileName)
    {
        var datetime = DateTime.Now;
        var outputPath = Path.Combine(
            baseDir,
            datetime.Year.ToString(),
            datetime.Month.ToString(),
            datetime.Day.ToString()
        );

        Directory.CreateDirectory(outputPath);
        var file = Path.Combine(outputPath, outputFileName);
        FFMpegArguments
            .FromUrlInput(
                uri,
                opts =>
                {
                    opts.WithCustomArgument("-y -rtsp_transport tcp");
                }
            )
            .OutputToFile(file, false, opts => opts.WithFrameOutputCount(1))
            .ProcessAsynchronously();
        return file;
    }
}
