using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using OpenTelemetryDemo.Domain.Abstractions;

namespace OpenTelemetryDemo.Infrastructure.Services.InMemory;

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    readonly ConcurrentDictionary<string, T> _repository = new();

    public Task<T?> FindAsync(string name, CancellationToken cancellationToken)
    {
        return _repository.TryGetValue(name, out var value)
            ? Task.FromResult<T?>(value)
            : Task.FromResult<T?>(default);
    }

    public Task AddOrUpdateAsync(string name, T value, CancellationToken cancellationToken)
    {
        _repository.AddOrUpdate(name, _ => value, (_,_) => value);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string name, CancellationToken cancellationToken)
    {
        _repository.TryRemove(name, out _);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<T> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var value in _repository.Values)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            yield return value;
        }
    }
}
