using Domain.Entities;

namespace Domain.Interfaces;

public interface IBetRepository
{
    Task<bool> ExistsByUserAndMatchAsync(int userId, int matchId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bet>> GetByMatchIdAsync(int matchId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bet>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Bet bet, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<Bet> bets, CancellationToken cancellationToken = default);
}
