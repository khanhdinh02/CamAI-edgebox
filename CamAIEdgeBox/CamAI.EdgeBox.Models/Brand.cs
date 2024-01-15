namespace CamAI.EdgeBox.Models;

public class Brand : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public LookUp BrandStatus { get; set; } = null!;
}