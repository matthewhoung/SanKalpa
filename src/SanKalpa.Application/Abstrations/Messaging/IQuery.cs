using MediatR;
using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Application.Abstrations.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
