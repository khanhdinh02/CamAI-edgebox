using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public class CameraService(UnitOfWork unitOfWork)
{
    public List<Camera> GetCamera() => unitOfWork.Cameras.GetAll();

    public Camera GetCamera(Guid id) => unitOfWork.Cameras.GetById(id) ?? throw new NullReferenceException($"Not found camera {id}");

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

    public void DeleteCamera(Guid id)
    {
        var camera = unitOfWork.Cameras.GetById(id);
        if (camera != null)
        {
            unitOfWork.Cameras.Delete(camera);
            unitOfWork.Complete();
        }
    }
}