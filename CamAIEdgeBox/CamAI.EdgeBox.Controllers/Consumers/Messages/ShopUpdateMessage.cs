using CamAI.EdgeBox.Models;
using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn(nameof(ShopUpdateMessage))]
public class ShopUpdateMessage : BaseUpdateMessage
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string Address { get; set; } = null!;

    public Shop ToShop() =>
        new()
        {
            Id = Id,
            Name = Name,
            Phone = Phone,
            Address = Address,
        };
}
