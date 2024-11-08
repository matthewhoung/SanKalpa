using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SanKalpa.Infrastructure.Authentication;

public class AuthenticationOptions
{
    public const string SectionName = "Authentication";
    // Jwt Validation Settings
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; }
    public string SecretKey { get; init; } = string.Empty;

    // Authentication scheme settings
    public string DefaultScheme { get; init; } = JwtBearerDefaults.AuthenticationScheme;
    public string DefaultChallengeScheme { get; init; } = JwtBearerDefaults.AuthenticationScheme;
    public string DefaultAuthenticateScheme { get; init; } = JwtBearerDefaults.AuthenticationScheme;
}
