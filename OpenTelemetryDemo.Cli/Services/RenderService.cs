using Microsoft.Extensions.Hosting;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Models;

namespace OpenTelemetryDemo.Cli.Services;

public class RenderService(IRepository<TrafficLight> repository) : BackgroundService
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
        Console.Clear();

        Console.WriteLine($"Process id: {Environment.ProcessId}");

        await foreach (var trafficLight in repository.GetAllAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"{trafficLight.Name}:");
            Console.WriteLine($"  Last transition: {trafficLight.LastTransition:u}");
            Console.WriteLine($"  State: {trafficLight.LightState}");
            Console.WriteLine($"  Queue: {trafficLight.QueuedTraffic}");
        }

    }
}
