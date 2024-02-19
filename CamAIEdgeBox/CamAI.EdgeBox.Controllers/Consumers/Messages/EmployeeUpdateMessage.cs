using CamAI.EdgeBox.Models;
using MassTransit;

namespace CamAI.EdgeBox.Consumers.Messages;

[MessageUrn(nameof(ShopUpdateMessage))]
public class EmployeeUpdateMessage : BaseUpdateMessage
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Gender Gender { get; set; }
    public string? Phone { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? AddressLine { get; set; }

    public Employee ToEmployee() =>
        new()
        {
            Id = Id,
            Name = Name,
            Phone = Phone,
            Email = Email,
            Gender = Gender,
            Birthday = Birthday,
            AddressLine = AddressLine
        };
}
