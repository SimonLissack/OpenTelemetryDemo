using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using OpenTelemetryDemo.Domain.Abstractions;
using OpenTelemetryDemo.Infrastructure.Instrumentation.Tracing;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryRepository<T>(ILogger<InMemoryRepository<T>> logger, Tracing tracing) : IRepository<T> where T : class
{
    readonly ConcurrentDictionary<string, T> _repository = new();

    public Task<T?> FindAsync(string name, CancellationToken cancellationToken)
    {
        using var activity = tracing.StartActivity("Find object");
        activity.SetActivityTags<T>(name);

        logger.LogDebug("Finding {Name}", name);

        return _repository.TryGetValue(name, out var value)
            ? Task.FromResult<T?>(value)
            : Task.FromResult<T?>(default);
    }

    public Task AddOrUpdateAsync(string name, T value, CancellationToken cancellationToken)
    {
        using var activity = tracing.StartActivity("Add or update object");
        activity.SetActivityTags<T>(name);

        logger.LogDebug("Adding {Name}", name);
        activity?.AddEvent(new ActivityEvent("Adding or updating item"));
        _repository.AddOrUpdate(name, _ => value, (_,_) => value);
        activity?.AddEvent(new ActivityEvent("Adding or update complete"));

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string name, CancellationToken cancellationToken)
    {
        using var activity = tracing.StartActivity("Removing object");
        activity.SetActivityTags<T>(name);

        logger.LogDebug("Removing {Name}", name);
        activity?.AddEvent(new ActivityEvent("Removing item"));

        _repository.TryRemove(name, out _);

        activity?.AddEvent(new ActivityEvent("Item removed"));

        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<T> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var activity = tracing.StartActivity("Getting all items");
        activity.SetActivityTags<T>();

        logger.LogInformation("Getting all items in repository");
        foreach (var value in _repository.Values)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Cancellation requested");
                yield break;
            }

            activity?.AddEvent(new ActivityEvent("yielding item"));
            yield return value;
        }

        activity?.AddEvent(new ActivityEvent("All items yielded"));
    }
}

file static class InMemoryRepositoryTelemetryExtensions
{
    public static void SetActivityTags<T>(this Activity? activity, string? itemName = null)
    {
        activity.TagObject<T>("repository_object");
        if (itemName != null)
        {
            activity?.SetTag("repository_key", itemName);
        }
    }
}
