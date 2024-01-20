using CamAI.EdgeBox.Models;
using CamAI.EdgeBox.Repositories;

namespace CamAI.EdgeBox.Services;

public class CameraService(UnitOfWork unitOfWork)
{
    public List<Camera> GetCamera()
    {
        return unitOfWork.Cameras.GetCameras();
    }
}