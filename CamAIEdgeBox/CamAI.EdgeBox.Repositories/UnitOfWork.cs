using CamAI.EdgeBox.Models;

namespace CamAI.EdgeBox.Repositories;

public class UnitOfWork(CamAiEdgeBoxContext db)
{
    private bool disposed;

    private CameraRepository? _cameraRepository;
    public CameraRepository Cameras => _cameraRepository ??= new CameraRepository(db);
    public async Task<int> CompleteAsync()
    {
        return await db.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;
        if (disposing)
        {
            db.Dispose();
        }
        disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        await db.DisposeAsync();
        Dispose(false);
        GC.SuppressFinalize(this);
    }
}