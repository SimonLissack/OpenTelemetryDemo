using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Infrastructure.Instrumentation;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Tracing;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryCommandDispatcher(ILogger<InMemoryCommandDispatcher> logger, MessagingMetrics metrics, Tracing tracing, CausationTracker causationTracker, ISender mediator) : ICommandDispatcher
{
    public async Task DispatchAsync<T>(T command, CancellationToken cancellationToken) where T : ICommand
    {
        using var activity = tracing.StartActivity("Dispatching command");
        activity?.TagObject<T>(TelemetryDefaults.TagNames.CommandName);

        logger.LogInformation("Dispatching command {CommandId}", command.Id);
        causationTracker.ApplyCausation(command);
        activity?.AddEvent(new ActivityEvent("Dispatching message"));

        await mediator.Send(command, cancellationToken);

        activity?.AddEvent(new ActivityEvent("Message dispatched"));
        metrics.CommandDispatched<T>();
        logger.LogInformation("Dispatched command {CommandId}", command.Id);
    }
}
