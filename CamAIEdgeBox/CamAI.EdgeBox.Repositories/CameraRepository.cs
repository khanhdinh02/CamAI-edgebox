using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public class CameraRepository(CamAiEdgeBoxContext db) : BaseRepository<Camera>(db);