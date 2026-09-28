using Application.DTOs.Matches;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;
using Match = Domain.Entities.Match;

namespace Test.Services;

public class MatchServiceTests
{
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IBetRepository> _betRepository = new();
    private readonly Mock<ITeamRepository> _teamRepository = new();
    private readonly MatchService _sut;

    public MatchServiceTests()
    {
        _sut = new MatchService(_matchRepository.Object, _betRepository.Object, _teamRepository.Object);
        _betRepository.Setup(r => r.GetByMatchIdAsync(It.IsAny<int>(), default)).ReturnsAsync([]);
        _teamRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync([]);
    }

    private static Team CreateTeamWithId(int id, string name)
    {
        var team = Team.Create(name);
        typeof(Team).GetProperty(nameof(Team.Id))!.SetValue(team, id);
        return team;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllMatchesMappedToDto()
    {
        var upcoming = Match.Create(100, 200);
        var finished = Match.Create(101, 201);
        finished.UpdateResult(3, 1);
        _matchRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync([upcoming, finished]);
        _teamRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(
        [
            CreateTeamWithId(100, "River Plate"),
            CreateTeamWithId(200, "Boca Juniors"),
            CreateTeamWithId(101, "Independiente"),
            CreateTeamWithId(201, "Racing Club")
        ]);

        var result = (await _sut.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(MatchStatus.UpcomingMatch, result[0].Status);
        Assert.Null(result[0].LocalGoals);
        Assert.Equal(new TeamDto(100, "River Plate"), result[0].LocalTeam);
        Assert.Equal(new TeamDto(200, "Boca Juniors"), result[0].VisitorTeam);
        Assert.Equal(MatchStatus.FullTime, result[1].Status);
        Assert.Equal(3, result[1].LocalGoals);
        Assert.Equal(1, result[1].VisitorGoals);
        Assert.Equal(new TeamDto(101, "Independiente"), result[1].LocalTeam);
        Assert.Equal(new TeamDto(201, "Racing Club"), result[1].VisitorTeam);
    }

    [Fact]
    public async Task GetAllAsync_OrdersUpcomingMatchesBeforeFinishedOnes()
    {
        var finished1 = Match.Create(101, 201);
        finished1.UpdateResult(2, 0);
        var finished2 = Match.Create(102, 202);
        finished2.UpdateResult(1, 1);
        var upcoming1 = Match.Create(103, 203);
        var upcoming2 = Match.Create(104, 204);
        _matchRepository.Setup(r => r.GetAllAsync(default))
            .ReturnsAsync([finished1, finished2, upcoming1, upcoming2]);
        _teamRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(
            Enumerable.Range(101, 4).Concat(Enumerable.Range(201, 4))
                .Select(id => CreateTeamWithId(id, $"Team {id}")));

        var result = (await _sut.GetAllAsync()).ToList();

        Assert.Equal(4, result.Count);
        Assert.All(result.Take(2), m => Assert.Equal(MatchStatus.UpcomingMatch, m.Status));
        Assert.All(result.Skip(2), m => Assert.Equal(MatchStatus.FullTime, m.Status));
    }

    [Fact]
    public async Task GetAllAsync_WhenNoMatchesExist_ReturnsEmptyCollection()
    {
        _matchRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync([]);

        var result = await _sut.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateResultAsync_WithExistingMatch_UpdatesGoalsAndPersists()
    {
        const int matchId = 10;
        var match = Match.Create(100, 200);
        var dto = new UpdateMatchResultDto(LocalGoals: 3, VisitorGoals: 1);
        _matchRepository.Setup(r => r.GetByIdAsync(matchId, default)).ReturnsAsync(match);

        var result = await _sut.UpdateResultAsync(matchId, dto);

        Assert.Equal(dto.LocalGoals, result.LocalGoals);
        Assert.Equal(dto.VisitorGoals, result.VisitorGoals);
        Assert.Equal(MatchStatus.FullTime, result.Status);
        _matchRepository.Verify(r => r.UpdateAsync(match, default), Times.Once);
    }

    [Fact]
    public async Task UpdateResultAsync_WhenMatchDoesNotExist_ThrowsDomainException()
    {
        var dto = new UpdateMatchResultDto(1, 0);
        _matchRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Match?)null);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _sut.UpdateResultAsync(999, dto));

        Assert.Equal("El partido no existe.", exception.Message);
        _matchRepository.Verify(r => r.UpdateAsync(It.IsAny<Match>(), default), Times.Never);
        _betRepository.Verify(r => r.GetByMatchIdAsync(It.IsAny<int>(), default), Times.Never);
    }

    [Fact]
    public async Task UpdateResultAsync_WithNegativeGoals_ThrowsDomainExceptionAndDoesNotPersist()
    {
        const int matchId = 10;
        var match = Match.Create(100, 200);
        var dto = new UpdateMatchResultDto(-1, 0);
        _matchRepository.Setup(r => r.GetByIdAsync(matchId, default)).ReturnsAsync(match);

        await Assert.ThrowsAsync<DomainException>(() => _sut.UpdateResultAsync(matchId, dto));

        _matchRepository.Verify(r => r.UpdateAsync(It.IsAny<Match>(), default), Times.Never);
        _betRepository.Verify(r => r.GetByMatchIdAsync(It.IsAny<int>(), default), Times.Never);
    }

    [Fact]
    public async Task UpdateResultAsync_WhenNoBetsExist_DoesNotCallUpdateRange()
    {
        const int matchId = 10;
        var match = Match.Create(100, 200);
        var dto = new UpdateMatchResultDto(2, 0);
        _matchRepository.Setup(r => r.GetByIdAsync(matchId, default)).ReturnsAsync(match);

        await _sut.UpdateResultAsync(matchId, dto);

        _betRepository.Verify(r => r.UpdateRangeAsync(It.IsAny<IEnumerable<Bet>>(), default), Times.Never);
    }

    [Fact]
    public async Task UpdateResultAsync_WithExactMatchBet_Awards3Points()
    {
        const int matchId = 10;
        var match = Match.Create(100, 200);
        var dto = new UpdateMatchResultDto(LocalGoals: 2, VisitorGoals: 1);
        var bet = Bet.Create(userId: 1, matchId, localGoals: 2, visitorGoals: 1);
        _matchRepository.Setup(r => r.GetByIdAsync(matchId, default)).ReturnsAsync(match);
        _betRepository.Setup(r => r.GetByMatchIdAsync(matchId, default)).ReturnsAsync([bet]);

        await _sut.UpdateResultAsync(matchId, dto);

        Assert.Equal(3, bet.PointsEarned);
        _betRepository.Verify(r => r.UpdateRangeAsync(
            It.Is<IEnumerable<Bet>>(bets => bets.Contains(bet)), default), Times.Once);
    }

    [Fact]
    public async Task UpdateResultAsync_WithCorrectTrendBet_Awards1Point()
    {
        const int matchId = 10;
        var match = Match.Create(100, 200);
        var dto = new UpdateMatchResultDto(LocalGoals: 3, VisitorGoals: 0);
        var bet = Bet.Create(userId: 1, matchId, localGoals: 1, visitorGoals: 0);
        _matchRepository.Setup(r => r.GetByIdAsync(matchId, default)).ReturnsAsync(match);
        _betRepository.Setup(r => r.GetByMatchIdAsync(matchId, default)).ReturnsAsync([bet]);

        await _sut.UpdateResultAsync(matchId, dto);

        Assert.Equal(1, bet.PointsEarned);
    }

    [Fact]
    public async Task UpdateResultAsync_WithWrongTrendBet_Awards0Points()
    {
        const int matchId = 10;
        var match = Match.Create(100, 200);
        var dto = new UpdateMatchResultDto(LocalGoals: 0, VisitorGoals: 2);
        var bet = Bet.Create(userId: 1, matchId, localGoals: 1, visitorGoals: 0);
        _matchRepository.Setup(r => r.GetByIdAsync(matchId, default)).ReturnsAsync(match);
        _betRepository.Setup(r => r.GetByMatchIdAsync(matchId, default)).ReturnsAsync([bet]);

        await _sut.UpdateResultAsync(matchId, dto);

        Assert.Equal(0, bet.PointsEarned);
    }

    [Fact]
    public async Task UpdateResultAsync_WithMultipleBets_RecalculatesPointsForEachOne()
    {
        const int matchId = 10;
        var match = Match.Create(100, 200);
        var dto = new UpdateMatchResultDto(LocalGoals: 2, VisitorGoals: 1);
        var exactBet = Bet.Create(userId: 1, matchId, localGoals: 2, visitorGoals: 1);
        var trendBet = Bet.Create(userId: 2, matchId, localGoals: 1, visitorGoals: 0);
        var wrongBet = Bet.Create(userId: 3, matchId, localGoals: 0, visitorGoals: 1);
        _matchRepository.Setup(r => r.GetByIdAsync(matchId, default)).ReturnsAsync(match);
        _betRepository.Setup(r => r.GetByMatchIdAsync(matchId, default))
            .ReturnsAsync([exactBet, trendBet, wrongBet]);

        await _sut.UpdateResultAsync(matchId, dto);

        Assert.Equal(3, exactBet.PointsEarned);
        Assert.Equal(1, trendBet.PointsEarned);
        Assert.Equal(0, wrongBet.PointsEarned);
        _betRepository.Verify(r => r.UpdateRangeAsync(
            It.Is<IEnumerable<Bet>>(bets => bets.Count() == 3), default), Times.Once);
    }
}
