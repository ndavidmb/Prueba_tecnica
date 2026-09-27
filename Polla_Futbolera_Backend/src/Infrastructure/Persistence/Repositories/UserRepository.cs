using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users.AnyAsync(u => u.Email == email, cancellationToken);
}
