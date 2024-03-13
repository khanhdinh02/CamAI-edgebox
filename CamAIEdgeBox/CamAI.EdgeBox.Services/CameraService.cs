﻿using CamAI.EdgeBox.Models;
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

    public List<Camera> GetCamera() => unitOfWork.Cameras.GetAll(false);

    public Camera GetCamera(Guid id) =>
        unitOfWork.Cameras.GetById(id)
        ?? throw new NullReferenceException($"Not found camera {id}");

    public Camera AddCamera(Camera camera)
    {
        var result = unitOfWork.Cameras.Add(camera);
        if (unitOfWork.Complete() > 0)
            GlobalData.Cameras = unitOfWork.Cameras.GetAll();
        return result;
    }

    public Camera UpdateCamera(Guid id, Camera camera)
    {
        camera.Id = id;
        unitOfWork.Cameras.Update(camera);
        if (unitOfWork.Complete() > 0)
            GlobalData.Cameras = unitOfWork.Cameras.GetAll();
        return camera;
    }

    // TODO: check camera health

    public void DeleteCamera(Guid id)
    {
        var camera = unitOfWork.Cameras.GetById(id);
        if (camera == null)
            return;

        unitOfWork.Cameras.Delete(camera);
        if (unitOfWork.Complete() > 0)
            GlobalData.Cameras = unitOfWork.Cameras.GetAll();
    }

    public FileStream GetM3U8File(Guid id)
    {
        var camera =
            unitOfWork.Cameras.GetById(id) ?? throw new NullReferenceException("Camera not found");

        var cameraName = id.ToString("N");
        var cameraDir = Path.Combine(streamingConfiguration.Directory, cameraName);
        if (!Directory.Exists(cameraDir))
            Directory.CreateDirectory(cameraDir);

        var m3u8File = StreamingEncoderProcessManager.RunEncoder(
            cameraName,
            camera.GetUri(),
            cameraDir
        );
        return File.OpenRead(Path.Combine(streamingConfiguration.Directory, m3u8File));
    }

    public FileStream GetTsFile(Guid id, string tsFileName)
    {
        var cameraName = id.ToString("N");
        StreamingEncoderProcessManager.UpdateTimer(cameraName);
        return File.OpenRead(
            Path.Combine(streamingConfiguration.Directory, cameraName, tsFileName)
        );
    }
}
