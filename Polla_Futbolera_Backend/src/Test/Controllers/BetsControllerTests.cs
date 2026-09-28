using System.Security.Claims;
using Application.DTOs.Bets;
using Application.DTOs.Matches;
using Application.Services;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace Test.Controllers;

public class BetsControllerTests
{
    private readonly Mock<IBetService> _betService = new();
    private readonly BetsController _sut;

    public BetsControllerTests()
    {
        _sut = new BetsController(_betService.Object);
        SetCurrentUser(userId: 1);
    }

    private void SetCurrentUser(int userId)
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())], "TestAuth");
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Fact]
    public async Task PlaceBet_WithValidBet_ReturnsCreatedWithResult()
    {
        var dto = new CreateBetDto(MatchId: 10, LocalGoals: 2, VisitorGoals: 1);
        var expected = new BetResultDto(1, 1, dto.MatchId, dto.LocalGoals, dto.VisitorGoals, null, null, 0, false, false);
        _betService.Setup(s => s.PlaceBetAsync(1, dto)).ReturnsAsync(expected);

        var result = await _sut.PlaceBet(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(expected, createdResult.Value);
    }

    [Fact]
    public async Task PlaceBet_WhenServiceThrowsDomainException_ReturnsConflict()
    {
        var dto = new CreateBetDto(MatchId: 10, LocalGoals: 2, VisitorGoals: 1);
        _betService.Setup(s => s.PlaceBetAsync(1, dto))
            .ThrowsAsync(new DomainException("Ya tienes una apuesta registrada para este partido."));

        var result = await _sut.PlaceBet(dto);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task PlaceBet_UsesUserIdFromNameIdentifierClaim()
    {
        SetCurrentUser(userId: 42);
        var dto = new CreateBetDto(MatchId: 10, LocalGoals: 1, VisitorGoals: 1);
        var expected = new BetResultDto(1, 42, dto.MatchId, dto.LocalGoals, dto.VisitorGoals, null, null, 0, false, false);
        _betService.Setup(s => s.PlaceBetAsync(42, dto)).ReturnsAsync(expected);

        await _sut.PlaceBet(dto);

        _betService.Verify(s => s.PlaceBetAsync(42, dto), Times.Once);
    }

    [Fact]
    public async Task GetMyBets_ReturnsOkWithUserBets()
    {
        var expected = new List<UserBetDto>
        {
            new(
                BetId: 1,
                MatchId: 10,
                LocalTeam: new TeamDto(100, "River Plate"),
                VisitorTeam: new TeamDto(200, "Boca Juniors"),
                PredictedLocalGoals: 2,
                PredictedVisitorGoals: 1,
                RealLocalGoals: null,
                RealVisitorGoals: null,
                MatchStatus: MatchStatus.UpcomingMatch,
                PointsEarned: 0)
        };
        _betService.Setup(s => s.GetUserBetsAsync(1)).ReturnsAsync(expected);

        var result = await _sut.GetMyBets();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetMyBets_UsesUserIdFromNameIdentifierClaim()
    {
        SetCurrentUser(userId: 42);
        _betService.Setup(s => s.GetUserBetsAsync(42)).ReturnsAsync([]);

        await _sut.GetMyBets();

        _betService.Verify(s => s.GetUserBetsAsync(42), Times.Once);
    }
}
