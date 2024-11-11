using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Domain.Services;

public sealed class LoginAttempt : Entity
{
    public string EmailAddress { get; private set; }
    public bool IsSuccessful { get; private set; }
    public DateTime AttemptedAt { get; private set; }
    public string? IpAddress { get; private set; }

    private LoginAttempt() { }

    private LoginAttempt(
        Guid id,
        string emailAddress,
        bool isSuccessful,
        string? ipAddress) : base(id)
    {
        EmailAddress = emailAddress;
        IsSuccessful = isSuccessful;
        AttemptedAt = DateTime.UtcNow;
        IpAddress = ipAddress;
    }

    public static LoginAttempt Create(
        string emailAddress,
        bool isSuccessful,
        string? ipAddress)
    {
        return new LoginAttempt(
            Guid.NewGuid(),
            emailAddress,
            isSuccessful,
            ipAddress);
    }
}
