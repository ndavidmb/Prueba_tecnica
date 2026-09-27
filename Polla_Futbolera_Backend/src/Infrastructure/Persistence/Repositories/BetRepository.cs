using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BetRepository(AppDbContext context) : IBetRepository
{
    public Task<bool> ExistsByUserAndMatchAsync(int userId, int matchId, CancellationToken cancellationToken = default) =>
        context.Bets.AnyAsync(b => b.UserId == userId && b.MatchId == matchId, cancellationToken);

    public async Task AddAsync(Bet bet, CancellationToken cancellationToken = default)
    {
        await context.Bets.AddAsync(bet, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
