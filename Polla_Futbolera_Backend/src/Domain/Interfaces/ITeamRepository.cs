using Domain.Entities;

namespace Domain.Interfaces;

public interface ITeamRepository
{
    Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default);
}
