namespace CamAI.EdgeBox.Models;

public class Shop : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? AddressLine { get; set; }
    public LookUp ShopLookUp { get; set; } = null!;
    public string Ward { get; set; } = null!;
}