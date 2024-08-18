using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Infrastructure.Instrumentation;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Tracing;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryEventDispatcher(ILogger<InMemoryEventDispatcher> logger, MessagingMetrics metrics, Tracing tracing, CausationTracker causationTracker, IPublisher publisher) : IEventDispatcher
{
    public async Task DispatchAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent
    {
        using var activity = tracing.StartActivity("Dispatching event");
        activity?.TagObject<T>(TelemetryDefaults.TagNames.EventName);

        logger.LogInformation("Dispatching Event {CommandId}", @event.Id);
        causationTracker.ApplyCausation(@event);
        activity?.AddEvent(new ActivityEvent("Dispatching event"));

        await publisher.Publish(@event, cancellationToken);

        activity?.AddEvent(new ActivityEvent("Event dispatched"));
        metrics.EventDispatched<T>();
        logger.LogInformation("Dispatched Event {CommandId}", @event.Id);
    }
}
