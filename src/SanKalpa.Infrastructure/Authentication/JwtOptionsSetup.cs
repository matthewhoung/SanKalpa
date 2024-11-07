using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace SanKalpa.Infrastructure.Authentication;

public class JwtOptionsSetup : IConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtOptionsSetup> _logger;

    public JwtOptionsSetup(IOptions<JwtOptions> options, ILogger<JwtOptionsSetup> logger)
    {
        _jwtOptions = options.Value;
        _logger = logger;
    }

    public void Configure(JwtBearerOptions options)
    {
        _logger.LogInformation("Configuring JWT Bearer Options with Issuer: {Issuer}, Audience: {Audience}",
            _jwtOptions.Issuer, _jwtOptions.Audience);

        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        if (secretKey.Key.Length < 32)
        {
            _logger.LogError("JWT Secret key is too short. Minimum length is 32 bytes.");
            throw new InvalidOperationException("JWT Secret key is too short");
        }

        options.TokenValidationParameters = new TokenValidationParameters
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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var token = context.Request.Headers["Authorization"].ToString();
                    if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Token = token["Bearer ".Length..].Trim();
                        _logger.LogInformation("JWT Token extracted successfully");
                    }
                    else
                    {
                        _logger.LogWarning("Authorization header found but doesn't start with 'Bearer '");
                    }
                }
                else
                {
                    _logger.LogWarning("No Authorization header found");
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                _logger.LogError(context.Exception, "Authentication failed");

                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                    _logger.LogWarning("Token expired");
                }
                else if (context.Exception is SecurityTokenInvalidSignatureException)
                {
                    _logger.LogWarning("Invalid token signature");
                }
                else if (context.Exception is SecurityTokenInvalidIssuerException)
                {
                    _logger.LogWarning("Invalid issuer");
                }
                else if (context.Exception is SecurityTokenInvalidAudienceException)
                {
                    _logger.LogWarning("Invalid audience");
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                _logger.LogInformation("Token validated successfully for subject: {Subject}",
                    context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return Task.CompletedTask;
            }
        };
    }
}
