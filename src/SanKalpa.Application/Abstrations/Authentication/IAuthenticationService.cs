using SanKalpa.Domain.Users;

namespace SanKalpa.Application.Abstrations.Authentication;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(User user, CancellationToken cancellationToken = default);
}
