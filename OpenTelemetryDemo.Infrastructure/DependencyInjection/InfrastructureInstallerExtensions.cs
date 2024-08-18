using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Domain.TrafficLights.Models;
using OpenTelemetryDemo.Infrastructure.Instrumentation;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Metrics;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Tracing;
using OpenTelemetryDemo.Infrastructure.Services;
using OpenTelemetryDemo.Infrastructure.Services.InMemory;

namespace OpenTelemetryDemo.Infrastructure.DependencyInjection;

public static class InfrastructureInstallerExtensions
{
    public static IServiceCollection AddHosting(this IServiceCollection services)
    {
        var infrastructureAssembly = Assembly.GetAssembly(typeof(InfrastructureInstallerExtensions))!;

        services
            .AddSingleton<IRepository<TrafficLight>, InMemoryRepository<TrafficLight>>()
            .AddScopedEventStore()
            .AddMediatR(c => c
                .RegisterServicesFromAssemblies(
                    infrastructureAssembly
                )
            )
            .AddOpenTelemetryInstrumentation()
            .AddMetricsInstrumentation();

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

    static IServiceCollection AddOpenTelemetryInstrumentation(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(c => c
                .AddService(TelemetryDefaults.RootMetricName)
                .AddTelemetrySdk()
                .AddAttributes([
                    TelemetryDefaults.Tags.MachineName
                ])
            )
            .AddTracingInstrumentation();

        return services;
    }

    static IServiceCollection AddMetricsInstrumentation(this IServiceCollection services)
    {
        services
            .AddMetrics()
            .AddSingleton<MessagingMetrics>()
            .AddSingleton<TrafficMetrics>();

        return services;
    }

    static OpenTelemetryBuilder AddTracingInstrumentation(this OpenTelemetryBuilder openTelemetryBuilder)
    {
        openTelemetryBuilder.Services
            .AddSingleton<Tracing>();

        openTelemetryBuilder
            .WithTracing(c => c
                .AddSource(TelemetryDefaults.RootName)
                .AddConsoleExporter(ce => ce.Targets = ConsoleExporterOutputTargets.Console)
            );

        return openTelemetryBuilder;
    }
}
