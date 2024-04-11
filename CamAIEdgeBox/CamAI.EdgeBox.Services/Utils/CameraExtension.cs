using System.Net.Sockets;
using System.Text;
using System.Web;
using CamAI.EdgeBox.Models;
using FFMpegCore;

namespace CamAI.EdgeBox.Services.Utils;

public static class CameraExtension
{
    public static string ToName(this Camera camera) => camera.Id.ToString("N");

    public static bool CanRunAI(this Camera camera) =>
        camera is { WillRunAI: true, Status: CameraStatus.Connected };

    public static Uri GetUri(this Camera camera)
    {
        var encodedUsername = HttpUtility.UrlEncode(camera.Username);
        var encodedPassword = HttpUtility.UrlEncode(camera.Password);
        return new Uri(
            $"{camera.Protocol}://{encodedUsername}:{encodedPassword}@{camera.Host}:{camera.Port}/{camera.Path}"
        );
    }

    /// <summary>
    /// Use DESCRIBE to check valid camera link
    /// </summary>
    /// <param name="camera"></param>
    /// <exception cref="Exception"></exception>
    public static void CheckConnection(this Camera camera)
    {
        var result = FFMpegArguments
            .FromUrlInput(camera.GetUri(), opts => opts.WithCustomArgument("-timeout 3000"))
            .OutputToFile(
                "-",
                false,
                options =>
                {
                    options.WithCustomArgument("-frames:v 1 -f null");
                }
            )
            .ProcessSynchronously();
        if (!result)
            throw new CameraConnectionException(camera);
    }

    private static string CreateRtspDescribeRequest(Camera camera)
    {
        var strBuilder = new StringBuilder();
        strBuilder.AppendLine($"DESCRIBE {camera.GetUri()} RTSP/1.0");
        strBuilder.AppendLine("CSeq: 1");
        strBuilder.AppendLine(
            $"Authorization: Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{camera.Username}:{camera.Password}"))}"
        );
        strBuilder.AppendLine();
        var message = strBuilder.ToString();
        return message;
    }
}
