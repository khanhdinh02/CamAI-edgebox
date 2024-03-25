using CamAI.EdgeBox.Models;
using FFMpegCore;

namespace CamAI.EdgeBox.Services.Utils;

public class RtspExtension(Camera camera, string baseDir)
{
    public Guid CameraId => camera.Id;
    private readonly Uri uri = camera.GetUri();

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

        // TODO: clean update image after one day
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
            .OutputToFile(
                file,
                false,
                opts => opts.WithFrameOutputCount(9).WithCustomArgument("-update 1")
            )
            .ProcessAsynchronously();
        return Path.Combine(dateTimeDir, outputFileName);
    }
}
