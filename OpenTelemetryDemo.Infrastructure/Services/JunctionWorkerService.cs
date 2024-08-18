using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Commands;
using OpenTelemetryDemo.Domain.TrafficLights.Models;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Tracing;

namespace OpenTelemetryDemo.Infrastructure.Services;

public class JunctionWorkerService(ILogger<JunctionWorkerService> logger, Tracing tracing, IServiceScopeFactory scopeFactory) : BackgroundService
{
    readonly TimeSpan _tickInterval = TimeSpan.FromSeconds(1);
    readonly Random _random = new();
    const int GreenLightTicks = 10;
    const int DeparturesPerTick = 10;
    const string Name = "Junction";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var state = await CreateJunction(stoppingToken);

        logger.LogInformation("Starting junction {Name}", Name);

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var trafficLight in state.GetTrafficLights())
            {
                using var activity = tracing.StartActivity("JunctionLightPhase");

                activity?.AddTag("traffic_light_name", trafficLight.Name);

                await using var lightTransitionScope = scopeFactory.CreateAsyncScope();
                var commandDispatcher = lightTransitionScope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

                logger.LogDebug("Starting green phase for {Name}", trafficLight.Name);

                activity?.AddEvent(new ActivityEvent("Setting light to green"));
                await commandDispatcher.DispatchAsync(
                    new RequestTransition
                    {
                        TrafficLightName = trafficLight.Name,
                        RequestedLightState = TrafficLightState.Green
                    },
                    stoppingToken
                );


                for (var i = 0; i < GreenLightTicks; i++)
                {
                    activity?.AddEvent(new ActivityEvent($"Tick {i}/{GreenLightTicks}"));
                    await commandDispatcher.DispatchAsync(
                        new RemoveLeavingTraffic
                        {
                            TrafficLightName = trafficLight.Name,
                            TrafficLeaving = trafficLight.DeparturesPerTick,
                            RaisedBy = Name
                        },
                        stoppingToken
                    );
                    await Tick(state, stoppingToken);
                }

                activity?.AddEvent(new ActivityEvent("Setting traffic light to Red"));
                await commandDispatcher.DispatchAsync(
                    new RequestTransition
                    {
                        TrafficLightName = trafficLight.Name,
                        RequestedLightState = TrafficLightState.Red
                    },
                    stoppingToken
                );
                logger.LogDebug("Green phase completed for {Name}", trafficLight.Name);
            }
        }

        logger.LogInformation("Stopping junction {Name}", Name);
    }

    async Task<JunctionState> CreateJunction(CancellationToken cancellationToken)
    {
        var state = new JunctionState();
        state.AddTrafficLight("north", 5, DeparturesPerTick);
        state.AddTrafficLight("east", 3, DeparturesPerTick);
        state.AddTrafficLight("south", 2, DeparturesPerTick);
        state.AddTrafficLight("west", 6, DeparturesPerTick);

        await using var createScope = scopeFactory.CreateAsyncScope();
        var commandDispatcher = createScope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        foreach (var trafficLight in state.GetTrafficLights())
        {
            await commandDispatcher.DispatchAsync(
                new AddTrafficLight
                {
                    TrafficLightName = trafficLight.Name,
                    RaisedBy = Name
                },
                cancellationToken
            );
        }

        return state;
    }

    async Task Tick(JunctionState state, CancellationToken cancellationToken)
    {
        await using var tickScope = scopeFactory.CreateAsyncScope();

        var commandDispatcher = tickScope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        var arrivalTasks = state.GetTrafficLights().Select(t =>
            commandDispatcher.DispatchAsync(new AddArrivingTraffic
            {
                TrafficLightName = t.Name,
                RaisedBy = Name,
                TrafficArrived = _random.Next(0, t.ArrivalsPerTick)
            }, cancellationToken)
        );

        await Task.WhenAll(arrivalTasks);
        await Task.Delay(_tickInterval, cancellationToken);
    }
}

internal class JunctionState
{
    List<TrafficLightDetails> TrafficLights { get; set; } = [];

    public void AddTrafficLight(string name, int arrivalsPerTick, int departuresPerTick)
    {
        TrafficLights.Add(new TrafficLightDetails
        {
            Name = name,
            ArrivalsPerTick = arrivalsPerTick,
            DeparturesPerTick = departuresPerTick
        });
    }

    public IEnumerable<TrafficLightDetails> GetTrafficLights() => TrafficLights;
}

internal class TrafficLightDetails
{
    public string Name { get; set; } = null!;
    public int ArrivalsPerTick { get; set; } = 0;
    public int DeparturesPerTick { get; set; } = 0;
}
