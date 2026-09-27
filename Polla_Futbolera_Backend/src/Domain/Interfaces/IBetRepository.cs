using Domain.Entities;

namespace Domain.Interfaces;

public interface IBetRepository
{
    Task<bool> ExistsByUserAndMatchAsync(int userId, int matchId, CancellationToken cancellationToken = default);
    Task AddAsync(Bet bet, CancellationToken cancellationToken = default);
}
