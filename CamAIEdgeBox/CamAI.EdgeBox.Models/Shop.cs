﻿namespace CamAI.EdgeBox.Models;

public class Shop : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string Address { get; set; } = null!;
}
