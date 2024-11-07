using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SanKalpa.Infrastructure.Authentication;

public sealed class JwtService : IJwtService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public string GenerateJwtToken(User user)
    {
        try
        {
            var secretKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

            var signingCredentials = new SigningCredentials(
                secretKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.EmailAddress.Value),
                new(ClaimTypes.Name, user.UserName.Value),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Add roles if you have them
                // new(ClaimTypes.Role, "user"),
            };

            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);

            var securityToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                expires: expires,
                claims: claims,
                signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            _logger.LogInformation("Generated JWT token for user {UserId}, expires at {ExpiryTime}",
                user.Id, expires);

            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
            throw;
        }
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Empty token provided for validation");
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = secretKey,
                ClockSkew = TimeSpan.Zero,
                RequireSignedTokens = true,
                RequireExpirationTime = true
            };

            var principal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Invalid token algorithm");
                return null;
            }

            _logger.LogInformation("Token validated successfully");
            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            _logger.LogWarning("Token has expired");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            _logger.LogWarning("Invalid token signature");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return null;
        }
    }
}
