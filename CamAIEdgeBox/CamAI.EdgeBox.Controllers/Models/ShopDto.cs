namespace CamAI.EdgeBox.Controllers.Models;

public class ShopDto
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string Address { get; set; } = null!;
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
    public bool IsShopOpen { get; set; }
}
