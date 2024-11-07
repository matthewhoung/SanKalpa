using Microsoft.EntityFrameworkCore;
using SanKalpa.Domain.Users;
using SanKalpa.Domain.Users.ValueObjects;

namespace SanKalpa.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailAddressAsync(
        string emailAddress, 
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<User>()
            .FirstOrDefaultAsync(
            user => user.EmailAddress.Value == emailAddress,
            cancellationToken);
    }
}
