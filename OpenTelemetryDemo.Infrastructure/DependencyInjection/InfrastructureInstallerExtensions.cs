using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Models;
using OpenTelemetryDemo.Infrastructure.Services;
using OpenTelemetryDemo.Infrastructure.Services.InMemory;

namespace OpenTelemetryDemo.Infrastructure.DependencyInjection;

public static class InfrastructureInstallerExtensions
{
    public static IServiceCollection AddHosting(this IServiceCollection services)
    {
        var infrastructureAssembly = Assembly.GetAssembly(typeof(InfrastructureInstallerExtensions))!;
        var domainAssembly = Assembly.GetAssembly(typeof(IMessage))!;

        services
            .AddSingleton<IRepository<TrafficLight>, InMemoryRepository<TrafficLight>>()
            .AddScopedEventStore()
            .AddMediatR(c => c
                .RegisterServicesFromAssemblies(
                    infrastructureAssembly,
                    domainAssembly
                )
            );

        return services;
    }

    static IServiceCollection AddScopedEventStore(this IServiceCollection services)
    {
        services
            .AddScoped<CausationTracker>()
            .AddScoped<ICommandDispatcher, InMemoryCommandDispatcher>()
            .AddScoped<IEventDispatcher, InMemoryEventDispatcher>();

        return services;
    }
}
