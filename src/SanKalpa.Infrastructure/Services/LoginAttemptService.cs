using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Domain.Services;

namespace SanKalpa.Infrastructure.Services;

internal sealed class LoginAttemptService : ILoginAttemptService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly IConfiguration _configuration;

    private const int MaxAttempts = 5;
    private const int LockoutMinutes = 5;

    public LoginAttemptService(
        ApplicationDbContext dbContext,
        IConfiguration configuration)
    {
        _dbcontext = dbContext;
        _configuration = configuration;
    }

    public async Task<bool> AttemptLoginAsync(
        string emailAddress,
        CancellationToken cancellationToken = default)
    {
        var recentFailedAttempts = await _dbcontext.Set<LoginAttempt>()
            .Where(la => la.EmailAddress == emailAddress
                && !la.IsSuccessful
                && la.AttemptedAt >= DateTime.UtcNow.AddMinutes(-LockoutMinutes))
            .CountAsync(cancellationToken);

        return recentFailedAttempts < MaxAttempts;

    }

    public async Task RecordLoginAttemptAsync(
        string emailAddress,
        bool isSuccessful,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        var loginAttempt = LoginAttempt.Create(emailAddress, isSuccessful, ipAddress);
        await _dbcontext.AddAsync(loginAttempt, cancellationToken);
        await _dbcontext.SaveChangesAsync(cancellationToken);

        // If login was successful, clear previous failed attempts
        if (isSuccessful)
        {
            var failedAttempts = await _dbcontext.Set<LoginAttempt>()
                .Where(la => la.EmailAddress == emailAddress && !la.IsSuccessful)
                .ToListAsync(cancellationToken);

            _dbcontext.RemoveRange(failedAttempts);
            await _dbcontext.SaveChangesAsync(cancellationToken);
        }
    }
}
