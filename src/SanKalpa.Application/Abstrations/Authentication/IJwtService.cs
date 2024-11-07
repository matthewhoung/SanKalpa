using SanKalpa.Domain.Users;
using System.Security.Claims;

namespace SanKalpa.Application.Abstrations.Authentication;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    ClaimsPrincipal? ValidateToken(string token);
}
