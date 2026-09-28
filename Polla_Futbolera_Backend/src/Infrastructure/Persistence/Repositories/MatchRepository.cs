using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MatchRepository(AppDbContext context) : IMatchRepository
{
    public async Task<IEnumerable<Match>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Matches.ToListAsync(cancellationToken);

    public Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Matches.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task UpdateAsync(Match match, CancellationToken cancellationToken = default)
    {
        context.Matches.Update(match);
        await context.SaveChangesAsync(cancellationToken);
    }
}
