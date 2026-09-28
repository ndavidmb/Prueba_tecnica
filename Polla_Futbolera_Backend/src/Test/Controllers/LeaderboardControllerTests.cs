using Application.DTOs.Leaderboard;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace Test.Controllers;

public class LeaderboardControllerTests
{
    private readonly Mock<ILeaderboardService> _leaderboardService = new();
    private readonly LeaderboardController _sut;

    public LeaderboardControllerTests()
    {
        _sut = new LeaderboardController(_leaderboardService.Object);
    }

    [Fact]
    public async Task GetLeaderboard_ReturnsOkWithEntries()
    {
        var entries = new List<UserLeaderboardDto>
        {
            new(UserId: 1, UserName: "Jane", TotalPoints: 9, TotalBets: 3, RankPosition: 1)
        };
        _leaderboardService.Setup(s => s.GetLeaderboardAsync()).ReturnsAsync(entries);

        var result = await _sut.GetLeaderboard();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(entries, okResult.Value);
    }

    [Fact]
    public async Task GetUserHistory_WhenUserExists_ReturnsOkWithHistory()
    {
        const int userId = 1;
        var history = new UserHistoryDto(userId, "Jane", TotalPoints: 6, Bets: []);
        _leaderboardService.Setup(s => s.GetUserHistoryAsync(userId)).ReturnsAsync(history);

        var result = await _sut.GetUserHistory(userId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(history, okResult.Value);
    }

    [Fact]
    public async Task GetUserHistory_WhenUserDoesNotExist_ReturnsNotFound()
    {
        const int userId = 404;
        _leaderboardService.Setup(s => s.GetUserHistoryAsync(userId))
            .ThrowsAsync(new DomainException("Usuario no encontrado."));

        var result = await _sut.GetUserHistory(userId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}
