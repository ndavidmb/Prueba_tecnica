using Application.DTOs.Bets;
using Application.DTOs.Matches;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using Match = Domain.Entities.Match;

namespace Test.Services;

public class BetServiceTests
{
    private readonly Mock<IBetRepository> _betRepository = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<ITeamRepository> _teamRepository = new();
    private readonly BetService _sut;

    public BetServiceTests()
    {
        _sut = new BetService(_betRepository.Object, _matchRepository.Object, _teamRepository.Object);
    }

    [Fact]
    public async Task PlaceBetAsync_WithPendingMatchAndNoPriorBet_CreatesBetWithZeroPoints()
    {
        const int userId = 1;
        var dto = new CreateBetDto(MatchId: 10, LocalGoals: 2, VisitorGoals: 1);
        var match = Match.Create(localTeamId: 100, visitorTeamId: 200);
        _matchRepository.Setup(r => r.GetByIdAsync(dto.MatchId, default)).ReturnsAsync(match);
        _betRepository.Setup(r => r.ExistsByUserAndMatchAsync(userId, dto.MatchId, default)).ReturnsAsync(false);

        var result = await _sut.PlaceBetAsync(userId, dto);

        Assert.Equal(userId, result.UserId);
        Assert.Equal(dto.MatchId, result.MatchId);
        Assert.Equal(dto.LocalGoals, result.PredictedLocalGoals);
        Assert.Equal(dto.VisitorGoals, result.PredictedVisitorGoals);
        Assert.Equal(0, result.PointsEarned);
        Assert.Null(result.RealLocalGoals);
        Assert.False(result.IsExactMatch);
        _betRepository.Verify(r => r.AddAsync(It.IsAny<Bet>(), default), Times.Once);
    }

    [Fact]
    public async Task PlaceBetAsync_WhenMatchDoesNotExist_ThrowsDomainException()
    {
        var dto = new CreateBetDto(MatchId: 999, LocalGoals: 1, VisitorGoals: 0);
        _matchRepository.Setup(r => r.GetByIdAsync(dto.MatchId, default)).ReturnsAsync((Match?)null);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _sut.PlaceBetAsync(1, dto));

        Assert.Equal("El partido no existe.", exception.Message);
        _betRepository.Verify(r => r.AddAsync(It.IsAny<Bet>(), default), Times.Never);
    }

    [Fact]
    public async Task PlaceBetAsync_WhenMatchIsAlreadyFinished_ThrowsDomainException()
    {
        var dto = new CreateBetDto(MatchId: 10, LocalGoals: 1, VisitorGoals: 0);
        var match = Match.Create(100, 200);
        match.UpdateResult(3, 1);
        _matchRepository.Setup(r => r.GetByIdAsync(dto.MatchId, default)).ReturnsAsync(match);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _sut.PlaceBetAsync(1, dto));

        Assert.Equal(
            "No se pueden realizar apuestas en un partido que no esté pendiente (UpcomingMatch).",
            exception.Message);
        _betRepository.Verify(r => r.AddAsync(It.IsAny<Bet>(), default), Times.Never);
    }

    [Fact]
    public async Task PlaceBetAsync_WhenUserAlreadyBetOnMatch_ThrowsDomainException()
    {
        const int userId = 1;
        var dto = new CreateBetDto(MatchId: 10, LocalGoals: 1, VisitorGoals: 0);
        var match = Match.Create(100, 200);
        _matchRepository.Setup(r => r.GetByIdAsync(dto.MatchId, default)).ReturnsAsync(match);
        _betRepository.Setup(r => r.ExistsByUserAndMatchAsync(userId, dto.MatchId, default)).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _sut.PlaceBetAsync(userId, dto));

        Assert.Equal("Ya tienes una apuesta registrada para este partido.", exception.Message);
        _betRepository.Verify(r => r.AddAsync(It.IsAny<Bet>(), default), Times.Never);
    }

    [Fact]
    public async Task PlaceBetAsync_WithNegativeGoals_ThrowsDomainException()
    {
        const int userId = 1;
        var dto = new CreateBetDto(MatchId: 10, LocalGoals: -1, VisitorGoals: 0);
        var match = Match.Create(100, 200);
        _matchRepository.Setup(r => r.GetByIdAsync(dto.MatchId, default)).ReturnsAsync(match);
        _betRepository.Setup(r => r.ExistsByUserAndMatchAsync(userId, dto.MatchId, default)).ReturnsAsync(false);

        await Assert.ThrowsAsync<DomainException>(() => _sut.PlaceBetAsync(userId, dto));
    }

    private static Team CreateTeamWithId(int id, string name)
    {
        var team = Team.Create(name);
        typeof(Team).GetProperty(nameof(Team.Id))!.SetValue(team, id);
        return team;
    }

    private static Match CreateMatchWithId(int id, int localTeamId, int visitorTeamId)
    {
        var match = Match.Create(localTeamId, visitorTeamId);
        typeof(Match).GetProperty(nameof(Match.Id))!.SetValue(match, id);
        return match;
    }

    [Fact]
    public async Task GetUserBetsAsync_ReturnsBetsWithMatchAndTeamNames()
    {
        const int userId = 1;
        const int matchId = 10;
        var match = CreateMatchWithId(matchId, 100, 200);
        var bet = Bet.Create(userId, matchId, localGoals: 2, visitorGoals: 1);
        _betRepository.Setup(r => r.GetByUserIdAsync(userId, default)).ReturnsAsync([bet]);
        _matchRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync([match]);
        _teamRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(
        [
            CreateTeamWithId(100, "River Plate"),
            CreateTeamWithId(200, "Boca Juniors")
        ]);

        var result = (await _sut.GetUserBetsAsync(userId)).ToList();

        Assert.Single(result);
        var dto = result[0];
        Assert.Equal(bet.Id, dto.BetId);
        Assert.Equal(matchId, dto.MatchId);
        Assert.Equal(new TeamDto(100, "River Plate"), dto.LocalTeam);
        Assert.Equal(new TeamDto(200, "Boca Juniors"), dto.VisitorTeam);
        Assert.Equal(2, dto.PredictedLocalGoals);
        Assert.Equal(1, dto.PredictedVisitorGoals);
        Assert.Null(dto.RealLocalGoals);
        Assert.Null(dto.RealVisitorGoals);
        Assert.Equal(MatchStatus.UpcomingMatch, dto.MatchStatus);
        Assert.Equal(0, dto.PointsEarned);
    }

    [Fact]
    public async Task GetUserBetsAsync_WithFinishedMatch_IncludesRealGoalsAndPoints()
    {
        const int userId = 1;
        const int matchId = 10;
        var match = CreateMatchWithId(matchId, 100, 200);
        match.UpdateResult(2, 1);
        var bet = Bet.Create(userId, matchId, localGoals: 2, visitorGoals: 1);
        bet.CalculateAndAssignPoints(2, 1);
        _betRepository.Setup(r => r.GetByUserIdAsync(userId, default)).ReturnsAsync([bet]);
        _matchRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync([match]);
        _teamRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(
        [
            CreateTeamWithId(100, "River Plate"),
            CreateTeamWithId(200, "Boca Juniors")
        ]);

        var result = (await _sut.GetUserBetsAsync(userId)).ToList();

        var dto = result[0];
        Assert.Equal(2, dto.RealLocalGoals);
        Assert.Equal(1, dto.RealVisitorGoals);
        Assert.Equal(MatchStatus.FullTime, dto.MatchStatus);
        Assert.Equal(3, dto.PointsEarned);
    }

    [Fact]
    public async Task GetUserBetsAsync_WhenUserHasNoBets_ReturnsEmptyCollection()
    {
        _betRepository.Setup(r => r.GetByUserIdAsync(1, default)).ReturnsAsync([]);
        _matchRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync([]);
        _teamRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync([]);

        var result = await _sut.GetUserBetsAsync(1);

        Assert.Empty(result);
    }
}
