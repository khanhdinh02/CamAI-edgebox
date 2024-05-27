using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services.Contracts;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Serilog;
using Action = CamAI.EdgeBox.Services.Contracts.Action;
using ArgumentException = System.ArgumentException;

namespace CamAI.EdgeBox.Services;

public class CameraService(IPublishEndpoint bus, AiService aiService)
{
    public Camera UpsertCamera(Camera camera)
    {
        var oldCamera = CameraRepository.GetById(camera.Id);
        if (oldCamera == null && GlobalData.MaxNumberOfRunningAi == GlobalData.Cameras?.Count)
        {
            Log.Warning(
                "The model {ModelName} can only run AI for {NumberOfCamera} cameras",
                GlobalData.EdgeBox?.Model,
                GlobalData.MaxNumberOfRunningAi
            );
            throw new ArgumentException(
                $"The model {GlobalData.EdgeBox?.Model} can only run AI for {GlobalData.MaxNumberOfRunningAi} cameras"
            );
        }
        var rerunAi =
            (
                oldCamera != null
                && (
                    camera.Port != oldCamera.Port
                    || camera.Host != oldCamera.Host
                    || camera.Protocol != oldCamera.Protocol
                    || camera.Username != oldCamera.Username
                    || camera.Password != oldCamera.Password
                    || camera.Path != oldCamera.Path
                )
            ) || ((oldCamera?.WillRunAI ?? false) && camera.WillRunAI);
        var duplicateCameras = GlobalData.Cameras?.Where(x =>
            x.Path == camera.Path
            && x.Port == camera.Port
            && x.Password == camera.Password
            && x.Username == camera.Username
            && x.Host == camera.Host
            && x.Protocol == camera.Protocol
            && x.Id != camera.Id
        );
        if (duplicateCameras?.Any() ?? false)
            throw new ArgumentException("Duplicate camera");

        UpdateCameraConnectionStatus(camera);
        CameraRepository.UpsertCamera(camera);
        camera.ShopId = GlobalData.Shop!.Id;
        bus.Publish(new CameraChangeMessage { Camera = camera, Action = Action.Upsert });
        if (rerunAi)
        {
            aiService.KillAi(camera);
            aiService.RunAi(camera);
        }
        return camera;
    }

    public void PublishCameraChangeMessage(Camera camera)
    {
        camera.ShopId = GlobalData.Shop!.Id;
        bus.Publish(new CameraChangeMessage { Camera = camera, Action = Action.Upsert });
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

    public void CheckCameraConnection(Guid id, bool runAi = true)
    {
        var camera = StaticCameraService.GetCamera(id);
        CheckCameraConnection(camera, runAi);
    }

    public void CheckCameraConnection(Camera camera, bool runAi = true)
    {
        UpdateCameraConnectionStatus(camera);
        CameraRepository.UpsertCamera(camera);
        GlobalData.Cameras = CameraRepository.GetAll();
        if (runAi)
            aiService.RunAi(camera);
    }

    public void DeleteCamera(Guid id)
    {
        var camera = CameraRepository.GetById(id);
        if (camera == null)
            return;

        aiService.KillAi(camera);
        GlobalData.Cameras = GlobalData.Cameras?.Where(x => x.Id != id).ToList() ?? [];
        bus.Publish(new CameraChangeMessage { Camera = camera, Action = Action.Delete });
    }
}

public static class StaticCameraService
{
    public static void UpsertCameraFromServerData(List<Camera> cameras)
    {
        foreach (var camera in cameras)
        {
            UpdateCameraConnectionStatus(camera);
        }

        GlobalData.Cameras = cameras;
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

    public static List<Camera> GetCamera() => CameraRepository.GetAll();

    public static Camera GetCamera(Guid id) =>
        CameraRepository.GetById(id) ?? throw new NullReferenceException($"Not found camera {id}");
}
