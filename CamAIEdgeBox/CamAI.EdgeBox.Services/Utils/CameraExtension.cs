using System.Web;
using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.Utils;

public static class CameraExtension
{
    public static Uri GetUri(this Camera camera)
    {
        var encodedUsername = HttpUtility.UrlEncode(camera.Username);
        var encodedPassword = HttpUtility.UrlEncode(camera.Password);
        return new Uri(
            $"{camera.Protocol}://{encodedUsername}:{encodedPassword}@{camera.Host}/{camera.Path}"
        );
    }
}
