using Domain.Entities;

namespace Domain.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Match match, CancellationToken cancellationToken = default);
}
