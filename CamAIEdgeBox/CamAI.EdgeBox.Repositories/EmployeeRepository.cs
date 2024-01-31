using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public class EmployeeRepository(CamAiEdgeBoxContext db) : BaseRepository<Employee>(db);
