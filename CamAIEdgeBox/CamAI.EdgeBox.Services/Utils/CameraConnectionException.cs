using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Services.Utils;

public class CameraConnectionException : Exception
{
    public CameraConnectionException(string message)
        : base(message) { }

    public CameraConnectionException(Camera camera)
        : base($"Cannot connect to camera link: {camera.GetUri()}") { }
}
