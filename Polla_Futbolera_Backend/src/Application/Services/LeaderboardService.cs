using Application.DTOs.Leaderboard;
using Application.Interfaces;
using Domain.Exceptions;

namespace Application.Services;

public class LeaderboardService(ILeaderboardRepository leaderboardRepository) : ILeaderboardService
{
    public Task<IEnumerable<UserLeaderboardDto>> GetLeaderboardAsync() =>
        leaderboardRepository.GetLeaderboardAsync();

    public async Task<UserHistoryDto> GetUserHistoryAsync(int userId)
    {
        var history = await leaderboardRepository.GetUserBetHistoryAsync(userId);

        return history ?? throw new DomainException("Usuario no encontrado.");
    }
}
