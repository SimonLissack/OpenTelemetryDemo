using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Models;

namespace OpenTelemetryDemo.Cli.Services;

public class LogRenderService(ILogger<LogRenderService> logger, IRepository<TrafficLight> repository) : BackgroundService
{
    readonly TimeSpan _sleepTimer = TimeSpan.FromSeconds(1);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Render(stoppingToken);
            await Task.Delay(_sleepTimer, stoppingToken);
        }
    }

    async Task Render(CancellationToken cancellationToken)
    {
        await foreach (var trafficLight in repository.GetAllAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogDebug("{TrafficLight} is {State} since {LastTransition} and has {QueuedCars} vehicles queued",
                trafficLight.Name,
                trafficLight.LightState,
                trafficLight.LastTransition,
                trafficLight.QueuedTraffic
            );
        }
    }
}
