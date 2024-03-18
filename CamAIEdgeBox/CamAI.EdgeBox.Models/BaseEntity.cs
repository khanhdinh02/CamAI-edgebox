using System.ComponentModel.DataAnnotations;

namespace CamAI.EdgeBox.Models;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}
