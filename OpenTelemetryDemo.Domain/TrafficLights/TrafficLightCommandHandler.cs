using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Commands;
using OpenTelemetryDemo.Domain.TrafficLights.Events;
using OpenTelemetryDemo.Domain.TrafficLights.Models;

namespace OpenTelemetryDemo.Domain.TrafficLights;

public class TrafficLightCommandHandler :
    ICommandHandler<AddTrafficLight>,
    ICommandHandler<RemoveTrafficLight>,
    ICommandHandler<RequestTransition>,
    ICommandHandler<AddArrivingTraffic>,
    ICommandHandler<RemoveLeavingTraffic>
{
    readonly IRepository<TrafficLight> _repository;
    readonly IEventDispatcher _eventDispatcher;

    public TrafficLightCommandHandler(IRepository<TrafficLight> repository, IEventDispatcher eventDispatcher)
    {
        _repository = repository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task Handle(AddTrafficLight command, CancellationToken cancellationToken)
    {
        var existingTrafficLight = await _repository.FindAsync(command.TrafficLightName, cancellationToken);

        if (existingTrafficLight is not null)
        {
            throw new TrafficLightAlreadyExistsException(command.TrafficLightName);
        }

        await _eventDispatcher.DispatchAsync(new TrafficLightAdded
        {
            TrafficLightName = command.TrafficLightName
        }, cancellationToken);
    }

    public async Task Handle(RemoveTrafficLight command, CancellationToken cancellationToken)
    {
        var existingTrafficLight = await _repository.FindAsync(command.TrafficLightName, cancellationToken);

        if (existingTrafficLight is not null)
        {
            await _eventDispatcher.DispatchAsync(new TrafficLightRemoved
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

        await _eventDispatcher.DispatchAsync(new TrafficLightTransitioned
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

        await _eventDispatcher.DispatchAsync(new TrafficLightQueuedTrafficChanged
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

        await _eventDispatcher.DispatchAsync(new TrafficLightQueuedTrafficChanged
        {
            TrafficLightName = command.TrafficLightName,
            NewTrafficCount = trafficCount

        }, cancellationToken);
    }

    async Task<TrafficLight> GetTrafficLight(string trafficLightName, CancellationToken cancellationToken)
    {
        var trafficLight = await _repository.FindAsync(trafficLightName, cancellationToken);

        if (trafficLight is null)
        {
            throw new TrafficLightDoesNotExistException(trafficLightName);
        }

        return trafficLight;
    }
}
