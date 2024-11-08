using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Domain.Users;

public sealed class RefreshToken : Entity
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private RefreshToken()
    {
    }

    private RefreshToken(
        string token,
        DateTime expiryDate,
        Guid userId)
    {
        Id = Guid.NewGuid();
        Token = token;
        ExpiryDate = expiryDate;
        UserId = userId;
        IsRevoked = false;
        CreatedAt = DateTime.UtcNow;
    }

    public static RefreshToken Create(
        string token,
        DateTime expiryDate,
        Guid userId)
    {
        return new RefreshToken(token, expiryDate, userId);
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
}
