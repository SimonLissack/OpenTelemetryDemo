using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryCommandDispatcher(ILogger<InMemoryCommandDispatcher> logger, MessagingMetrics metrics, CausationTracker causationTracker, ISender mediator) : ICommandDispatcher
{
    public async Task DispatchAsync<T>(T command, CancellationToken cancellationToken) where T : ICommand
    {
        logger.LogInformation("Dispatching command {CommandId}", command.Id);
        causationTracker.ApplyCausation(command);
        await mediator.Send(command, cancellationToken);
        metrics.CommandDispatched<T>();
        logger.LogInformation("Dispatched command {CommandId}", command.Id);
    }
}
