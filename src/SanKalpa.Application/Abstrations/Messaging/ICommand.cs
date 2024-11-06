using MediatR;
using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Application.Abstrations.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}

public interface ICommand<TReponse> : IRequest<Result<TReponse>>, IBaseCommand
{
}

public interface IBaseCommand
{
}
