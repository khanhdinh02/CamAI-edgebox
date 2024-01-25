using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public class ShopRepository(CamAiEdgeBoxContext db) : BaseRepository<Shop>(db);
