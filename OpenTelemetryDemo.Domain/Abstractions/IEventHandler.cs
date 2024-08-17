using MediatR;

namespace OpenTelemetryDemo.Domain.Abstractions;

public interface IEventHandler<in T> : INotificationHandler<T> where T : IEvent
{
}
