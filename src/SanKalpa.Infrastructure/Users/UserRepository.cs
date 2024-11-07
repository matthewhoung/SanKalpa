using Microsoft.EntityFrameworkCore;
using SanKalpa.Domain.Users;

namespace SanKalpa.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<User>().FirstOrDefaultAsync(user => user.EmailAddress.Value == email, cancellationToken);
    }
}
