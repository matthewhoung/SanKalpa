namespace SanKalpa.Application.Abstrations.Authentication;

public interface ILoginAttemptService
{
    Task<bool> AttemptLoginAsync(
        string emailAddress, 
        CancellationToken cancellation = default);
    Task RecordLoginAttemptAsync(
        string emailAddress,
        bool isSuccessful,
        string? ipAddress,
        CancellationToken cancellationToken = default);
}
