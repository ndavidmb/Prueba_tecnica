using Domain.Entities;
using Domain.Exceptions;

namespace Test.Domain;

public class BetTests
{
    [Fact]
    public void Create_WithValidGoals_ReturnsBetWithZeroPoints()
    {
        var bet = Bet.Create(userId: 1, matchId: 2, localGoals: 2, visitorGoals: 1);

        Assert.Equal(1, bet.UserId);
        Assert.Equal(2, bet.MatchId);
        Assert.Equal(2, bet.LocalGoals);
        Assert.Equal(1, bet.VisitorGoals);
        Assert.Equal(0, bet.PointsEarned);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-3, -3)]
    public void Create_WithNegativeGoals_ThrowsDomainException(int localGoals, int visitorGoals)
    {
        var exception = Assert.Throws<DomainException>(() => Bet.Create(1, 2, localGoals, visitorGoals));

        Assert.Equal("Los goles pronosticados no pueden ser negativos.", exception.Message);
    }

    [Fact]
    public void CalculateAndAssignPoints_WhenExactMatch_Awards3Points()
    {
        var bet = Bet.Create(1, 2, localGoals: 2, visitorGoals: 1);

        bet.CalculateAndAssignPoints(realLocalGoals: 2, realVisitorGoals: 1);

        Assert.Equal(3, bet.PointsEarned);
    }

    [Theory]
    [InlineData(2, 1, 3, 0)]
    [InlineData(0, 2, 1, 3)]
    [InlineData(1, 1, 2, 2)]
    public void CalculateAndAssignPoints_WhenTrendMatchesButNotExact_Awards1Point(
        int predictedLocal, int predictedVisitor, int realLocal, int realVisitor)
    {
        var bet = Bet.Create(1, 2, predictedLocal, predictedVisitor);

        bet.CalculateAndAssignPoints(realLocal, realVisitor);

        Assert.Equal(1, bet.PointsEarned);
    }

    [Theory]
    [InlineData(2, 1, 0, 1)]
    [InlineData(1, 1, 2, 0)]
    [InlineData(0, 3, 1, 1)]
    public void CalculateAndAssignPoints_WhenTrendDoesNotMatch_Awards0Points(
        int predictedLocal, int predictedVisitor, int realLocal, int realVisitor)
    {
        var bet = Bet.Create(1, 2, predictedLocal, predictedVisitor);

        bet.CalculateAndAssignPoints(realLocal, realVisitor);

        Assert.Equal(0, bet.PointsEarned);
    }
}
