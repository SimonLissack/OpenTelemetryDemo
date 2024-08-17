namespace OpenTelemetryDemo.Domain.Abstractions;

public interface IEventHandler<in T> where T : IEvent
{
    public Task HandleAsync(T @event, CancellationToken cancellationToken);
}
