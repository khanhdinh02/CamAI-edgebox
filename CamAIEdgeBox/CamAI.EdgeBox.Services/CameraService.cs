﻿using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;
using CamAI.EdgeBox.Services.Contracts;
using CamAI.EdgeBox.Services.Utils;
using MassTransit;
using Action = CamAI.EdgeBox.Services.Contracts.Action;

namespace CamAI.EdgeBox.Services;

public class CameraService(IPublishEndpoint bus, AiService aiService)
{
    public Camera UpsertCamera(Camera camera)
    {
        UpdateCameraConnectionStatus(camera);
        CameraRepository.UpsertCamera(camera);
        GlobalData.Cameras = CameraRepository.GetAll();
        bus.Publish(new CameraChangeMessage { Camera = camera, Action = Action.Upsert });
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

        CameraRepository.DeleteCamera(id);
        GlobalData.Cameras = CameraRepository.GetAll();
        bus.Publish(
            new CameraChangeMessage
            {
                Camera = new Camera { Id = id },
                Action = Action.Delete
            }
        );
    }
}

public static class StaticCameraService
{
    public static Camera UpsertCameraFromServerData(Camera camera)
    {
        UpdateCameraConnectionStatus(camera);
        CameraRepository.UpsertCamera(camera);
        GlobalData.Cameras = CameraRepository.GetAll();
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

    public static List<Camera> GetCamera() => CameraRepository.GetAll();

    public static Camera GetCamera(Guid id) =>
        CameraRepository.GetById(id) ?? throw new NullReferenceException($"Not found camera {id}");
}
