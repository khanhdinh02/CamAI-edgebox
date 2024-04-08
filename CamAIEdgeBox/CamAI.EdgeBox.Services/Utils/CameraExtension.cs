using System.Net.Sockets;
using System.Text;
using System.Web;
using CamAI.EdgeBox.Models;

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
            $"{camera.Protocol}://{encodedUsername}:{encodedPassword}@{camera.Host}/{camera.Path}"
        );
    }

    /// <summary>
    /// Use DESCRIBE to check valid camera link
    /// </summary>
    /// <param name="camera"></param>
    /// <exception cref="Exception"></exception>
    public static void CheckConnection(this Camera camera)
    {
        // init client
        using var client = new TcpClient(camera.Host, camera.Port);
        var nwStream = client.GetStream();

        // send describe request
        var describeRequestBytes = Encoding.ASCII.GetBytes(CreateRtspDescribeRequest(camera));
        nwStream.Write(describeRequestBytes, 0, describeRequestBytes.Length);

        // receive response
        nwStream.ReadTimeout = 5000;
        var bytesToRead = new byte[client.ReceiveBufferSize];
        var bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
        var response = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

        // read response
        using var reader = new StringReader(response);
        var header = reader.ReadLine()!;
        if (!header.Contains("200"))
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
