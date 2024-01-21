using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public class EdgeBoxRepository(CamAiEdgeBoxContext db) : BaseRepository<DbEdgeBox>(db);
