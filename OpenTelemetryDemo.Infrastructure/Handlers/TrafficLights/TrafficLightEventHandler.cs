using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Events;
using OpenTelemetryDemo.Domain.TrafficLights.Models;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

namespace OpenTelemetryDemo.Infrastructure.Handlers.TrafficLights;

public class TrafficLightEventHandler(ILogger<TrafficLightEventHandler> logger, IRepository<TrafficLight> repository, TrafficMetrics trafficMetrics) :
    IEventHandler<TrafficLightAdded>,
    IEventHandler<TrafficLightRemoved>,
    IEventHandler<TrafficLightTransitioned>,
    IEventHandler<TrafficLightQueuedTrafficChanged>
{
    public async Task Handle(TrafficLightAdded @event, CancellationToken cancellationToken)
    {
        var trafficLight = new TrafficLight
        {
            Name = @event.TrafficLightName,
            LightState = TrafficLightState.Red
        };

        await repository.AddOrUpdateAsync(@event.TrafficLightName, trafficLight, cancellationToken);
    }

    public async Task Handle(TrafficLightRemoved @event, CancellationToken cancellationToken)
    {
        logger.LogDebug("Removing traffic light {Name}", @event.TrafficLightName);
        await repository.RemoveAsync(@event.TrafficLightName, cancellationToken);
    }

    public async Task Handle(TrafficLightTransitioned @event, CancellationToken cancellationToken)
    {
        logger.LogDebug("Transitioning traffic light {Name} to {NewState}", @event.TrafficLightName, @event.TrafficLightState);
        var trafficLight = await repository.FindAsync(@event.TrafficLightName, cancellationToken);

        trafficLight!.LightState = @event.TrafficLightState;
        trafficLight.LastTransition = DateTime.Now;

        await repository.AddOrUpdateAsync(@event.TrafficLightName, trafficLight, cancellationToken);
    }

    public async Task Handle(TrafficLightQueuedTrafficChanged @event, CancellationToken cancellationToken)
    {
        logger.LogDebug("Setting traffic light {Name} queued vehicle count to {Size}", @event.TrafficLightName, @event.NewTrafficCount);

        var trafficLight = await repository.FindAsync(@event.TrafficLightName, cancellationToken);

        trafficLight!.QueuedTraffic = @event.NewTrafficCount;

        await repository.AddOrUpdateAsync(@event.TrafficLightName, trafficLight, cancellationToken);

        trafficMetrics.SetQueueDepth(trafficLight.Name, trafficLight.QueuedTraffic);
    }
}
