using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services.Contracts;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;

namespace CamAI.EdgeBox.Services;

public class CameraService(IPublishEndpoint bus, AiService aiService)
{
    public List<Camera> GetCamera() => CameraRepository.GetAll();

    public Camera GetCamera(Guid id) =>
        CameraRepository.GetById(id) ?? throw new NullReferenceException($"Not found camera {id}");

    public Camera UpsertCamera(Camera camera)
    {
        UpdateCameraConnectionStatus(camera);
        CameraRepository.UpsertCamera(camera);
        GlobalData.Cameras = CameraRepository.GetAll();
        bus.Publish(CameraChangeMessage.ToUpsertMessage(camera));
        aiService.RunAi(camera);
        return camera;
    }

    private static void UpdateCameraConnectionStatus(Camera camera)
    {
        try
        {
            camera.CheckConnection();
            camera.Status = CameraStatus.Connected;
        }
        catch (Exception)
        {
            camera.Status = CameraStatus.Disconnected;
        }
    }

    public void CheckCameraConnection(Guid id)
    {
        var camera = GetCamera(id);
        UpdateCameraConnectionStatus(camera);
        CameraRepository.UpsertCamera(camera);
        aiService.RunAi(camera);
    }

    public void DeleteCamera(Guid id)
    {
        var camera = CameraRepository.GetById(id);
        if (camera == null)
            return;

        CameraRepository.DeleteCamera(id);
        GlobalData.Cameras = CameraRepository.GetAll();
        bus.Publish(CameraChangeMessage.ToDeleteMessage(id));
    }
}
