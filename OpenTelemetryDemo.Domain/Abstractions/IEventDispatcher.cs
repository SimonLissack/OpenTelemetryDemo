namespace OpenTelemetryDemo.Domain.Abstractions;

public interface IEventDispatcher
{
    public Task DispatchAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent;
}
