namespace SanKalpa.Application.Abstrations.Authentication;

public interface IRefreshTokenService
{
    Task<string> RefreshTokenGeneratorAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);
    Task<(string AccessToken,string RefreshToken)> RefreshAccessTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default);
}
