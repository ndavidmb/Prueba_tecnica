using Application.DTOs.Leaderboard;

namespace Application.Interfaces;

public interface ILeaderboardRepository
{
    Task<IEnumerable<UserLeaderboardDto>> GetLeaderboardAsync(CancellationToken cancellationToken = default);
    Task<UserHistoryDto?> GetUserBetHistoryAsync(int userId, CancellationToken cancellationToken = default);
}
