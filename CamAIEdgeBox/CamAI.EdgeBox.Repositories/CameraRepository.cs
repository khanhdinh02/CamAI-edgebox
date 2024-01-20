using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public class CameraRepository(CamAiEdgeBoxContext db)
{
    public List<Camera> GetCameras()
    {
        return db.Cameras.ToList();
    }
    // get
    // get all
    // update
    // create
    // delete
}