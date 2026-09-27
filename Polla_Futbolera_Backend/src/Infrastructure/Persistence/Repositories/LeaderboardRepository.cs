using Application.DTOs.Leaderboard;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class LeaderboardRepository(AppDbContext context) : ILeaderboardRepository
{
    public async Task<IEnumerable<UserLeaderboardDto>> GetLeaderboardAsync(CancellationToken cancellationToken = default)
    {
        var rows = await context.Users
            .GroupJoin(
                context.Bets,
                u => u.Id,
                b => b.UserId,
                (u, bets) => new
                {
                    UserId = u.Id,
                    UserName = u.Name,
                    TotalPoints = bets.Sum(b => (int?)b.PointsEarned) ?? 0,
                    TotalBets = bets.Count()
                })
            .OrderByDescending(x => x.TotalPoints)
            .ToListAsync(cancellationToken);

        return rows.Select((x, i) => new UserLeaderboardDto(
            x.UserId,
            x.UserName,
            x.TotalPoints,
            x.TotalBets,
            RankPosition: i + 1));
    }

    public async Task<UserHistoryDto?> GetUserBetHistoryAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null) return null;

        var bets = await context.Bets
            .Where(b => b.UserId == userId)
            .Join(context.Matches, b => b.MatchId, m => m.Id, (b, m) => new { Bet = b, Match = m })
            .Join(context.Teams, x => x.Match.LocalTeamId, t => t.Id, (x, t) => new { x.Bet, x.Match, LocalTeam = t })
            .Join(context.Teams, x => x.Match.VisitorTeamId, t => t.Id, (x, t) => new UserBetHistoryItemDto(
                x.Bet.Id,
                x.Bet.MatchId,
                x.LocalTeam.Name,
                t.Name,
                x.Bet.LocalGoals,
                x.Bet.VisitorGoals,
                x.Match.LocalGoals,
                x.Match.VisitorGoals,
                x.Bet.PointsEarned))
            .ToListAsync(cancellationToken);

        return new UserHistoryDto(
            user.Id,
            user.Name,
            bets.Sum(b => b.PointsEarned),
            bets);
    }
}
