using CamAI.EdgeBox.Models;
using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn(nameof(ShopUpdateMessage))]
public class ShopUpdateMessage : BaseUpdateMessage
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string Address { get; set; } = null!;
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }

    public Shop ToShop() =>
        new()
        {
            Id = Id,
            Name = Name,
            Phone = Phone,
            Address = Address,
            OpenTime = OpenTime,
            CloseTime = CloseTime
        };
}
