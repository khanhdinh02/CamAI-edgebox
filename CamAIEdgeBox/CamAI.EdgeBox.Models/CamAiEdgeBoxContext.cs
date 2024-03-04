using Microsoft.EntityFrameworkCore;

namespace CamAI.EdgeBox.Models;

public class CamAiEdgeBoxContext : DbContext
{
    public DbSet<Camera> Cameras { get; set; } = null!;
    public DbSet<Shop> Shops { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<DbEdgeBox> EdgeBoxes { get; set; } = null!;

    private string DbPath { get; }

    public CamAiEdgeBoxContext()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        DbPath = Path.Join(path, $"edgeBox_{Environment.MachineName}.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
}
