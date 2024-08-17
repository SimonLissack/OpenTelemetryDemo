﻿using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Events;
using OpenTelemetryDemo.Domain.TrafficLights.Models;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;

namespace OpenTelemetryDemo.Infrastructure.Handlers.TrafficLights;

public class TrafficLightEventHandler(IRepository<TrafficLight> repository, TrafficMetrics trafficMetrics) :
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
        await repository.RemoveAsync(@event.TrafficLightName, cancellationToken);
    }

    public async Task Handle(TrafficLightTransitioned @event, CancellationToken cancellationToken)
    {
        var trafficLight = await repository.FindAsync(@event.TrafficLightName, cancellationToken);

        trafficLight!.LightState = @event.TrafficLightState;
        trafficLight.LastTransition = DateTime.Now;

        await repository.AddOrUpdateAsync(@event.TrafficLightName, trafficLight, cancellationToken);
    }

    public async Task Handle(TrafficLightQueuedTrafficChanged @event, CancellationToken cancellationToken)
    {
        var trafficLight = await repository.FindAsync(@event.TrafficLightName, cancellationToken);

        trafficLight!.QueuedTraffic = @event.NewTrafficCount;

        await repository.AddOrUpdateAsync(@event.TrafficLightName, trafficLight, cancellationToken);

        trafficMetrics.UpdateQueuedTraffic(trafficLight.Name, trafficLight.QueuedTraffic, trafficLight.LightState);
    }
}