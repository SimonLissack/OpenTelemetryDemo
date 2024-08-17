using MediatR;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryEventDispatcher(MessagingMetrics metrics, CausationTracker causationTracker, IPublisher publisher) : IEventDispatcher
{
    public async Task DispatchAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent
    {
        causationTracker.ApplyCausation(@event);
        await publisher.Publish(@event, cancellationToken);
        metrics.EventDispatched<T>();
    }
}
