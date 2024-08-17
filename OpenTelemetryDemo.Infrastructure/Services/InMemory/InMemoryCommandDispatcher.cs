﻿using MediatR;
using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryCommandDispatcher(CausationTracker causationTracker, IMediator mediator) : ICommandDispatcher
{
    public async Task DispatchAsync<T>(T command, CancellationToken cancellationToken) where T : ICommand
    {
        causationTracker.ApplyCausation(command);
        await mediator.Publish(command, cancellationToken);
    }
}
