using Application.DTOs.Leaderboard;

namespace Application.Services;

public interface ILeaderboardService
{
    Task<IEnumerable<UserLeaderboardDto>> GetLeaderboardAsync();
    Task<UserHistoryDto> GetUserHistoryAsync(int userId);
}
