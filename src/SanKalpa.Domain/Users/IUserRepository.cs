using SanKalpa.Domain.Users.ValueObjects;

namespace SanKalpa.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string emailAddress, CancellationToken cancellationToken = default);
    void Add(User user);
}
