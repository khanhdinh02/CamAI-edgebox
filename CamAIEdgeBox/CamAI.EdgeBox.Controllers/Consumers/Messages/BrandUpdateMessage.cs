using CamAI.EdgeBox.Models;
using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn(nameof(BrandUpdateMessage))]
public class BrandUpdateMessage
{
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public Brand ToBrand() =>
        new()
        {
            Name = Name,
            Email = Email,
            Phone = Phone
        };
}
