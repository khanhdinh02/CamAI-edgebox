using FFMpegCore;

namespace CamAI.EdgeBox.Services.Utils;

public class RtspExtension(Uri uri, string baseDir)
{
    public string CaptureFrame(string outputFileName)
    {
        outputFileName += ".png";
        var datetime = DateTime.Now;
        var dateTimeDir = Path.Combine(
            datetime.Year.ToString(),
            datetime.Month.ToString(),
            datetime.Day.ToString()
        );
        var outputPath = Path.Combine(baseDir, dateTimeDir);

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
        return Path.Combine(dateTimeDir, outputFileName);
    }
}
