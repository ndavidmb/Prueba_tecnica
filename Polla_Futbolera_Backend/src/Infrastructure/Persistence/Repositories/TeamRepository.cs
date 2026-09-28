using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TeamRepository(AppDbContext context) : ITeamRepository
{
    public async Task<IEnumerable<Team>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Teams.ToListAsync(cancellationToken);
}
