using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Application.Abstrations.Authentication;

public interface IJwtService
{
    Result<string> TokenGenerator(
        Guid userId,
        string userName,
        string emailAddress,
        string password,
        CancellationToken cancellationToken = default);
}
