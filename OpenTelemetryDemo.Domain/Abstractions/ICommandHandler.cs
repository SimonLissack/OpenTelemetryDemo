namespace OpenTelemetryDemo.Domain.Abstractions;

public interface ICommandHandler<in T> where T : ICommand
{
    public Task Handle(T command, CancellationToken cancellationToken);
}
