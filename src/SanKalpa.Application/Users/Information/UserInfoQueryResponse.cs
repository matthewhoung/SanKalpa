namespace SanKalpa.Application.Users.Information;

public sealed record class UserInfoQueryResponse
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string EmailAddress { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
