using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public class BrandRepository(CamAiEdgeBoxContext db) : BaseRepository<Brand>(db);