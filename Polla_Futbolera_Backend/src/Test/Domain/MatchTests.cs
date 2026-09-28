using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Test.Domain;

public class MatchTests
{
    [Fact]
    public void Create_WithDifferentTeams_ReturnsUpcomingMatch()
    {
        var match = Match.Create(localTeamId: 1, visitorTeamId: 2);

        Assert.Equal(1, match.LocalTeamId);
        Assert.Equal(2, match.VisitorTeamId);
        Assert.Equal(MatchStatus.UpcomingMatch, match.Status);
        Assert.Null(match.LocalGoals);
        Assert.Null(match.VisitorGoals);
    }

    [Fact]
    public void Create_WithSameTeamOnBothSides_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => Match.Create(1, 1));

        Assert.Equal("Un equipo no puede jugar contra sí mismo.", exception.Message);
    }

    [Fact]
    public void UpdateResult_WithValidGoals_SetsGoalsAndFullTimeStatus()
    {
        var match = Match.Create(1, 2);

        match.UpdateResult(3, 1);

        Assert.Equal(3, match.LocalGoals);
        Assert.Equal(1, match.VisitorGoals);
        Assert.Equal(MatchStatus.FullTime, match.Status);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    public void UpdateResult_WithNegativeGoals_ThrowsDomainException(int localGoals, int visitorGoals)
    {
        var match = Match.Create(1, 2);

        var exception = Assert.Throws<DomainException>(() => match.UpdateResult(localGoals, visitorGoals));

        Assert.Equal("Los goles no pueden ser negativos.", exception.Message);
        Assert.Equal(MatchStatus.UpcomingMatch, match.Status);
    }
}
