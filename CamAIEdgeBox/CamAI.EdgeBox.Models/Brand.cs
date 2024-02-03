namespace CamAI.EdgeBox.Models;

public class Brand : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    // TODO [Duy]: how about logo and banner
}
