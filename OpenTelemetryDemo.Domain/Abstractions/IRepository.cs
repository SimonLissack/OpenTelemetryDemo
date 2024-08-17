namespace OpenTelemetryDemo.Domain.Abstractions;

public interface IRepository<T>
{
    public Task<T?> FindAsync(string name, CancellationToken cancellationToken);
    public Task AddOrUpdateAsync(string name, T value, CancellationToken cancellationToken);
    public Task RemoveAsync(string name, CancellationToken cancellationToken);
    public IAsyncEnumerable<T> GetAllAsync(CancellationToken cancellationToken);
}
