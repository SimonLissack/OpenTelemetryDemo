namespace OpenTelemetryDemo.Domain.Abstractions;

public abstract class Command : ICommand
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CausationId { get; set; }
    public string RaisedBy { get; set; } = null!;
    public DateTime RaisedAt { get; set; } = DateTime.UtcNow;
}
