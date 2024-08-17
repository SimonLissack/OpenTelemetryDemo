namespace OpenTelemetryDemo.Domain.Abstractions;

public interface ICommandDispatcher
{
    public Task DispatchAsync<T>(T command) where T : ICommand;
}
