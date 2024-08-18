using MediatR;

namespace OpenTelemetryDemo.Domain.Abstractions;

public interface ICommandHandler<in T> : IRequestHandler<T> where T : ICommand
{
}
