using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetryDemo.Cli.Services;
using OpenTelemetryDemo.Infrastructure.DependencyInjection;
using OpenTelemetryDemo.Infrastructure.Services;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureServices(s => s
    .AddHosting()
    .AddHostedService<JunctionWorkerService>()
    .AddHostedService<LogRenderService>()
);

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, eventArgs) =>
{
    cts.Cancel();
    eventArgs.Cancel = true;
};

var host = hostBuilder.Build();

await host.StartAsync(cts.Token);
await host.WaitForShutdownAsync(cts.Token);
