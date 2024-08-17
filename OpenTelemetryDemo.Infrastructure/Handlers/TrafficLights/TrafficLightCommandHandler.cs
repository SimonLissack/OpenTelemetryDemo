using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Commands;
using OpenTelemetryDemo.Domain.TrafficLights.Events;
using OpenTelemetryDemo.Domain.TrafficLights.Models;

namespace OpenTelemetryDemo.Infrastructure.Handlers.TrafficLights;

public class TrafficLightCommandHandler(ILogger<TrafficLightCommandHandler> logger, IRepository<TrafficLight> repository, IEventDispatcher eventDispatcher) :
    ICommandHandler<AddTrafficLight>,
    ICommandHandler<RemoveTrafficLight>,
    ICommandHandler<RequestTransition>,
    ICommandHandler<AddArrivingTraffic>,
    ICommandHandler<RemoveLeavingTraffic>
{
    public async Task Handle(AddTrafficLight command, CancellationToken cancellationToken)
    {
        var existingTrafficLight = await repository.FindAsync(command.TrafficLightName, cancellationToken);

        if (existingTrafficLight is not null)
        {
            throw new TrafficLightAlreadyExistsException(command.TrafficLightName);
        }

        await eventDispatcher.DispatchAsync(new TrafficLightAdded
        {
            TrafficLightName = command.TrafficLightName
        }, cancellationToken);
    }

    public async Task Handle(RemoveTrafficLight command, CancellationToken cancellationToken)
    {
        var existingTrafficLight = await repository.FindAsync(command.TrafficLightName, cancellationToken);

        if (existingTrafficLight is not null)
        {
            await eventDispatcher.DispatchAsync(new TrafficLightRemoved
            {
                TrafficLightName = command.TrafficLightName
            }, cancellationToken);
        }
    }

    public async Task Handle(RequestTransition command, CancellationToken cancellationToken)
    {
        var trafficLight = await GetTrafficLight(command.TrafficLightName, cancellationToken);

        if (trafficLight.LightState == command.RequestedLightState)
        {
            return;
        }

        await eventDispatcher.DispatchAsync(new TrafficLightTransitioned
        {
            TrafficLightName = command.TrafficLightName,
            TrafficLightState = command.RequestedLightState
        }, cancellationToken);
    }

    public async Task Handle(AddArrivingTraffic command, CancellationToken cancellationToken)
    {
        if (command.TrafficArrived <= 0)
        {
            return;
        }

        var trafficLight = await GetTrafficLight(command.TrafficLightName, cancellationToken);

        var trafficCount = trafficLight.QueuedTraffic + command.TrafficArrived;

        await eventDispatcher.DispatchAsync(new TrafficLightQueuedTrafficChanged
        {
            TrafficLightName = command.TrafficLightName,
            NewTrafficCount = trafficCount

        }, cancellationToken);
    }

    public async Task Handle(RemoveLeavingTraffic command, CancellationToken cancellationToken)
    {
        if (command.TrafficLeaving <= 0)
        {
            return;
        }

        var trafficLight = await GetTrafficLight(command.TrafficLightName, cancellationToken);

        var trafficCount = trafficLight.QueuedTraffic - command.TrafficLeaving;

        trafficCount = trafficCount < 0
            ? 0
            : trafficCount;

        await eventDispatcher.DispatchAsync(new TrafficLightQueuedTrafficChanged
        {
            TrafficLightName = command.TrafficLightName,
            NewTrafficCount = trafficCount

        }, cancellationToken);
    }

    async Task<TrafficLight> GetTrafficLight(string trafficLightName, CancellationToken cancellationToken)
    {
        var trafficLight = await repository.FindAsync(trafficLightName, cancellationToken);

        if (trafficLight is null)
        {
            throw new TrafficLightDoesNotExistException(trafficLightName);
        }

        return trafficLight;
    }
}
