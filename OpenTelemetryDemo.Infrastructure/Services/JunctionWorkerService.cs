using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Commands;
using OpenTelemetryDemo.Domain.TrafficLights.Models;

namespace OpenTelemetryDemo.Infrastructure.Services;

public class JunctionWorkerService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    readonly TimeSpan _tickInterval = TimeSpan.FromSeconds(1);
    const int GreenLightTicks = 10;
    const string Name = "Junction";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var state = await CreateJunction(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var trafficLight in state.GetTrafficLights())
            {
                await using var lightTransitionScope = scopeFactory.CreateAsyncScope();
                var commandDispatcher = lightTransitionScope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

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
                    await Tick(state, stoppingToken);
                }

                await commandDispatcher.DispatchAsync(
                    new RequestTransition
                    {
                        TrafficLightName = trafficLight.Name,
                        RequestedLightState = TrafficLightState.Red
                    },
                    stoppingToken
                );
            }
        }
    }

    async Task<JunctionState> CreateJunction(CancellationToken cancellationToken)
    {
        var state = new JunctionState();
        state.AddTrafficLight("north", 5, 6);
        state.AddTrafficLight("east", 3, 1);
        state.AddTrafficLight("south", 2, 2);
        state.AddTrafficLight("west", 6, 10);

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
                TrafficArrived = t.ArrivalsPerTick
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
