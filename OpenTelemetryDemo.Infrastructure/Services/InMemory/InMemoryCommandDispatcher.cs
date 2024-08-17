using MediatR;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryCommandDispatcher(MessagingMetrics metrics, CausationTracker causationTracker, ISender mediator) : ICommandDispatcher
{
    public async Task DispatchAsync<T>(T command, CancellationToken cancellationToken) where T : ICommand
    {
        causationTracker.ApplyCausation(command);
        await mediator.Send(command, cancellationToken);
        metrics.CommandDispatched<T>();
    }
}
