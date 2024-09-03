using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
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
            .AddOpenTelemetryInstrumentation();

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
                .AddEnvironmentVariableDetector()
                .AddTelemetrySdk()
                .AddAttributes([
                    TelemetryDefaults.Tags.MachineName
                ])
            )
            .UseOtlpExporter()
            .AddLoggingInstrumentation()
            .AddTracingInstrumentation()
            .AddMetricsInstrumentation();

        return services;
    }

    static IOpenTelemetryBuilder AddLoggingInstrumentation(this IOpenTelemetryBuilder openTelemetryBuilder)
    {
        openTelemetryBuilder
            .WithLogging();

        return openTelemetryBuilder;
    }

    static IOpenTelemetryBuilder AddMetricsInstrumentation(this IOpenTelemetryBuilder openTelemetryBuilder)
    {
        openTelemetryBuilder.Services
            .AddMetrics()
            .AddSingleton<MessagingMetrics>()
            .AddSingleton<TrafficMetrics>();

        openTelemetryBuilder
            .WithMetrics(b => b
                .AddMeter(MessagingMetrics.MeterName)
                .AddMeter(TrafficMetrics.MeterName)
            );

        return openTelemetryBuilder;
    }

    static IOpenTelemetryBuilder AddTracingInstrumentation(this IOpenTelemetryBuilder openTelemetryBuilder)
    {
        openTelemetryBuilder.Services
            .AddSingleton<Tracing>();

        openTelemetryBuilder
            .WithTracing(b => b
                .AddSource(TelemetryDefaults.RootName)
            );

        return openTelemetryBuilder;
    }
}
