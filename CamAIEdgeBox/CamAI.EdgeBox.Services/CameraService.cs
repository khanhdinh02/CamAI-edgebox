using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services.Streaming;
using CamAI.EdgeBox.Services.Utils;
using Microsoft.Extensions.Options;

namespace CamAI.EdgeBox.Services;

public class CameraService(
    IOptions<StreamingConfiguration> streamingConfiguration,
    UnitOfWork unitOfWork
)
{
    private readonly StreamingConfiguration streamingConfiguration = streamingConfiguration.Value;

    public List<Camera> GetCamera() => unitOfWork.Cameras.GetAll();

    public Camera GetCamera(Guid id) =>
        unitOfWork.Cameras.GetById(id)
        ?? throw new NullReferenceException($"Not found camera {id}");

    public Camera AddCamera(Camera camera)
    {
        var result = unitOfWork.Cameras.Add(camera);
        unitOfWork.Complete();
        return result;
    }

    public Camera UpdateCamera(Guid id, Camera camera)
    {
        camera.Id = id;
        unitOfWork.Cameras.Update(camera);
        unitOfWork.Complete();
        return camera;
    }

    // TODO: check camera health
    // TODO: run ai for camera

    public void DeleteCamera(Guid id)
    {
        var camera = unitOfWork.Cameras.GetById(id);
        if (camera != null)
        {
            unitOfWork.Cameras.Delete(camera);
            unitOfWork.Complete();
        }
    }

    public FileStream GetM3U8File(Guid id)
    {
        var camera =
            unitOfWork.Cameras.GetById(id) ?? throw new NullReferenceException("Camera not found");

        var cameraName = id.ToString("N");
        var cameraDir = Path.Combine(streamingConfiguration.Directory, cameraName);
        if (!Directory.Exists(cameraDir))
            Directory.CreateDirectory(cameraDir);

        var cameraFileName = Path.Combine(cameraDir, $"{cameraName}.m3u8");
        StreamingEncoderProcessManager.RunEncoder(cameraDir, camera.GetUri(), cameraFileName);
        Thread.Sleep(5000);
        return File.OpenRead(Path.Combine(streamingConfiguration.Directory, cameraFileName));
    }

    public FileStream GetTsFile(Guid id, string tsFileName)
    {
        var cameraName = id.ToString("N");
        return File.OpenRead(
            Path.Combine(streamingConfiguration.Directory, cameraName, tsFileName)
        );
    }
}
