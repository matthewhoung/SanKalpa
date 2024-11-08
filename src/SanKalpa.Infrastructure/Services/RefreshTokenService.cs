using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Domain.Users;
using SanKalpa.Infrastructure.Authentication;
using System.Security.Cryptography;

namespace SanKalpa.Infrastructure.Services;

internal class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly JwtOptions _jwtOptions;

    public RefreshTokenService(
        ApplicationDbContext dbContext,
        IJwtService jwtService,
        IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _jwtOptions = jwtOptions.Value;
    }
    public async Task<string> RefreshTokenGeneratorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        string refreshToken = Convert.ToBase64String(randomNumber);

        var refreshTokenEntity = RefreshToken.Create(
            refreshToken,
            DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays),
            userId);

        await _dbContext.AddAsync(refreshTokenEntity);
        _dbContext.SaveChanges();

        return refreshToken;
    }

    public async Task<(string AccessToken, string RefreshToken)> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedRefreshToken = await _dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(token => token.Token == refreshToken, cancellationToken);

        if (storedRefreshToken == null)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        if (storedRefreshToken.ExpiryDate < DateTime.UtcNow || storedRefreshToken.IsRevoked)
        {
            throw new InvalidOperationException("Refresh token expired or revoked");
        }

        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == storedRefreshToken.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var newAccessToken = _jwtService.TokenGenerator(
            user.Id,
            user.UserName.Value,
            user.EmailAddress.Value,
            user.Password.Value,
            cancellationToken);

        var newRefreshToken = await RefreshTokenGeneratorAsync(storedRefreshToken.UserId);

        storedRefreshToken.Revoke();
        _dbContext.SaveChanges();

        if (newAccessToken.IsFailure)
        {
            throw new InvalidOperationException("Failed to generate access token");
        }

        return (newAccessToken.Value, newRefreshToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedRefreshToken = await _dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(token => token.Token == refreshToken, cancellationToken);

        if (storedRefreshToken != null)
        {
            storedRefreshToken.Revoke();
            _dbContext.SaveChanges();
        }
    }
}
