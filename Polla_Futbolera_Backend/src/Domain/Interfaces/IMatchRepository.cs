using Domain.Entities;

namespace Domain.Interfaces;

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Match?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Match match, CancellationToken cancellationToken = default);
}
