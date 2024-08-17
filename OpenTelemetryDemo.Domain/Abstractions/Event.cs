using MediatR;

namespace OpenTelemetryDemo.Domain.Abstractions;

public interface IEvent : INotification
{
    public Guid Id { get; set; }
    public Guid CausationId { get; set; }
    public DateTime RaisedAt { get; set; }
}

public class Event : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CausationId { get; set; }
    public DateTime RaisedAt { get; set; } = DateTime.UtcNow;
}
