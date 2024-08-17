using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Events;
using OpenTelemetryDemo.Domain.TrafficLights.Models;

namespace OpenTelemetryDemo.Domain.TrafficLights;

public class TrafficLightEventHandler :
    IEventHandler<TrafficLightAdded>,
    IEventHandler<TrafficLightRemoved>,
    IEventHandler<TrafficLightTransitioned>,
    IEventHandler<TrafficLightQueuedTrafficChanged>
{
    readonly IRepository<TrafficLight> _repository;

    public TrafficLightEventHandler(IRepository<TrafficLight> repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(TrafficLightAdded @event, CancellationToken cancellationToken)
    {
        var trafficLight = new TrafficLight
        {
            Name = @event.TrafficLightName,
            LightState = TrafficLightState.Red
        };

        await _repository.AddOrUpdateAsync(@event.TrafficLightName, trafficLight, cancellationToken);
    }

    public async Task HandleAsync(TrafficLightRemoved @event, CancellationToken cancellationToken)
    {
        await _repository.RemoveAsync(@event.TrafficLightName, cancellationToken);
    }

    public async Task HandleAsync(TrafficLightTransitioned @event, CancellationToken cancellationToken)
    {
        var trafficLight = await _repository.FindAsync(@event.TrafficLightName, cancellationToken);

        trafficLight!.LightState = @event.TrafficLightState;

        await _repository.AddOrUpdateAsync(@event.TrafficLightName, trafficLight, cancellationToken);
    }

    public async Task HandleAsync(TrafficLightQueuedTrafficChanged @event, CancellationToken cancellationToken)
    {
        var trafficLight = await _repository.FindAsync(@event.TrafficLightName, cancellationToken);

        trafficLight!.QueuedTraffic = @event.NewTrafficCount;

        await _repository.AddOrUpdateAsync(@event.TrafficLightName, trafficLight, cancellationToken);
    }
}
