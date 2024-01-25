namespace CamAI.EdgeBox.MassTransit;

public class RabbitMqConfiguration
{
    public string HostName { get; set; } = null!;
    public string? VirtualHost { get; set; }
    public ushort Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
