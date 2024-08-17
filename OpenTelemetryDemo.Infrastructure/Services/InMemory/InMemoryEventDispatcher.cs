using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryEventDispatcher(ILogger<InMemoryEventDispatcher> logger, MessagingMetrics metrics, CausationTracker causationTracker, IPublisher publisher) : IEventDispatcher
{
    public async Task DispatchAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent
    {
        logger.LogInformation("Dispatching Event {CommandId}", @event.Id);
        causationTracker.ApplyCausation(@event);
        await publisher.Publish(@event, cancellationToken);
        metrics.EventDispatched<T>();
        logger.LogInformation("Dispatched Event {CommandId}", @event.Id);
    }
}
