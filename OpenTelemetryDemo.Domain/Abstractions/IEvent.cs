using MediatR;

namespace OpenTelemetryDemo.Domain.Abstractions;

public interface IEvent : IMessage, INotification
{
    public DateTime RaisedAt { get; set; }
}
