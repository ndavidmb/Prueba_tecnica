using Application.DTOs.Leaderboard;
using Application.Interfaces;
using Application.Services;
using Domain.Exceptions;
using Moq;

namespace Test.Services;

public class LeaderboardServiceTests
{
    private readonly Mock<ILeaderboardRepository> _leaderboardRepository = new();
    private readonly LeaderboardService _sut;

    public LeaderboardServiceTests()
    {
        _sut = new LeaderboardService(_leaderboardRepository.Object);
    }

    [Fact]
    public async Task GetLeaderboardAsync_ReturnsRepositoryResult()
    {
        var entries = new List<UserLeaderboardDto>
        {
            new(UserId: 1, UserName: "Jane", TotalPoints: 9, TotalBets: 3, RankPosition: 1),
            new(UserId: 2, UserName: "John", TotalPoints: 6, TotalBets: 3, RankPosition: 2)
        };
        _leaderboardRepository.Setup(r => r.GetLeaderboardAsync(default)).ReturnsAsync(entries);

        var result = await _sut.GetLeaderboardAsync();

        Assert.Equal(entries, result);
    }

    [Fact]
    public async Task GetUserHistoryAsync_WhenUserExists_ReturnsHistory()
    {
        const int userId = 1;
        var history = new UserHistoryDto(userId, "Jane", TotalPoints: 6, Bets: []);
        _leaderboardRepository.Setup(r => r.GetUserBetHistoryAsync(userId, default)).ReturnsAsync(history);

        var result = await _sut.GetUserHistoryAsync(userId);

        Assert.Equal(history, result);
    }

    [Fact]
    public async Task GetUserHistoryAsync_WhenUserDoesNotExist_ThrowsDomainException()
    {
        const int userId = 404;
        _leaderboardRepository.Setup(r => r.GetUserBetHistoryAsync(userId, default)).ReturnsAsync((UserHistoryDto?)null);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _sut.GetUserHistoryAsync(userId));

        Assert.Equal("Usuario no encontrado.", exception.Message);
    }
}
