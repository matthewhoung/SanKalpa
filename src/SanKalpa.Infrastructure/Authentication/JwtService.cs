using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Domain.Abstrations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SanKalpa.Infrastructure.Authentication;

public sealed class JwtService : IJwtService
{
    private static readonly Error AuthenticationFailed = new(
        "Authentication.Failed",
        "Failed to acquire token due to authentication failure.");

    private readonly JwtOptions _jwtOptions;

    public JwtService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public Result<string> TokenGenerator(
        Guid userId,
        string userName,
        string emailAddress, 
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, emailAddress),
            new(JwtRegisteredClaimNames.UniqueName, userName),
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Result.Success(tokenString);
        }
        catch
        {
            return Result.Failure<string>(AuthenticationFailed);
        }
    }
}
