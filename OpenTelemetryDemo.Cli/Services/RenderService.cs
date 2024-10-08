﻿using Microsoft.Extensions.Hosting;
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
        var baseColor = Console.ForegroundColor;
        Console.Clear();

        Console.WriteLine($"Process id: {Environment.ProcessId}");

        await foreach (var trafficLight in repository.GetAllAsync(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"{trafficLight.Name}:");
            Console.WriteLine($"  Last transition: {trafficLight.LastTransition:u}");
            Console.Write("  State: ");
            var lightColor = trafficLight.LightState == TrafficLightState.Green ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = lightColor;
            Console.WriteLine(trafficLight.LightState);
            Console.ForegroundColor = baseColor;
            Console.WriteLine($"  Queue: {trafficLight.QueuedTraffic}");
        }
    }
}
