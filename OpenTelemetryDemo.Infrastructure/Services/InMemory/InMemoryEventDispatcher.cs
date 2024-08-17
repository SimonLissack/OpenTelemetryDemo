using MediatR;
using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryEventDispatcher(CausationTracker causationTracker, IPublisher publisher) : IEventDispatcher
{
    public async Task DispatchAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent
    {
        causationTracker.ApplyCausation(@event);
        await publisher.Publish(@event, cancellationToken);
    }
}
