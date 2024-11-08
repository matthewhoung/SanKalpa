﻿namespace SanKalpa.Infrastructure.Authentication;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; }
    public int RefreshTokenExpiryDays { get; init; }
    public string SecretKey { get; init; } = string.Empty;
}
