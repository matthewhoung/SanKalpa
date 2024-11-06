using MediatR;
using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Application.Abstrations.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
