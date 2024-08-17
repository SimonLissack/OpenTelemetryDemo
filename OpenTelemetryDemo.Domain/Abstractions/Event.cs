namespace OpenTelemetryDemo.Domain.Abstractions;

public class Event : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CausationId { get; set; }
    public Guid CorrelationId { get; set; }
    public DateTime RaisedAt { get; set; } = DateTime.UtcNow;
}
