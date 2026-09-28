using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BetRepository(AppDbContext context) : IBetRepository
{
    public Task<bool> ExistsByUserAndMatchAsync(int userId, int matchId, CancellationToken cancellationToken = default) =>
        context.Bets.AnyAsync(b => b.UserId == userId && b.MatchId == matchId, cancellationToken);

    public async Task<IEnumerable<Bet>> GetByMatchIdAsync(int matchId, CancellationToken cancellationToken = default) =>
        await context.Bets.Where(b => b.MatchId == matchId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Bet>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        await context.Bets.Where(b => b.UserId == userId).ToListAsync(cancellationToken);

    public async Task AddAsync(Bet bet, CancellationToken cancellationToken = default)
    {
        await context.Bets.AddAsync(bet, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<Bet> bets, CancellationToken cancellationToken = default)
    {
        context.Bets.UpdateRange(bets);
        await context.SaveChangesAsync(cancellationToken);
    }
}
