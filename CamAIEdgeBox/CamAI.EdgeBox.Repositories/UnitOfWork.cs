using CamAI.EdgeBox.Models;
using Microsoft.EntityFrameworkCore;

namespace CamAI.EdgeBox.Repositories;

public class UnitOfWork(CamAiEdgeBoxContext db)
{
    private bool disposed;

    private CameraRepository? _cameraRepository;
    public CameraRepository Cameras => _cameraRepository ??= new CameraRepository(db);

    private BrandRepository? _brandRepository;
    public BrandRepository Brands => _brandRepository ??= new BrandRepository(db);
    private ShopRepository? _shopRepository;
    public ShopRepository Shops => _shopRepository ??= new ShopRepository(db);
    private EdgeBoxRepository? _edgeBoxRepository;
    public EdgeBoxRepository EdgeBoxes => _edgeBoxRepository ??= new EdgeBoxRepository(db);

    public int Complete() => db.SaveChanges();

    public void Detach(object entity) => db.Entry(entity).State = EntityState.Detached;

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
}
