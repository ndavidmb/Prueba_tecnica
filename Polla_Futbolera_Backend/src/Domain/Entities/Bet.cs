using Domain.Exceptions;

namespace Domain.Entities;

public class Bet
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public int MatchId { get; private set; }
    public int LocalGoals { get; private set; }
    public int VisitorGoals { get; private set; }
    public int PointsEarned { get; private set; }

    private Bet() { }

    public static Bet Create(int userId, int matchId, int localGoals, int visitorGoals)
    {
        if (localGoals < 0 || visitorGoals < 0)
            throw new DomainException("Los goles pronosticados no pueden ser negativos.");

        return new Bet
        {
            UserId = userId,
            MatchId = matchId,
            LocalGoals = localGoals,
            VisitorGoals = visitorGoals,
            PointsEarned = 0
        };
    }

    public void CalculateAndAssignPoints(int realLocalGoals, int realVisitorGoals)
    {
        if (LocalGoals == realLocalGoals && VisitorGoals == realVisitorGoals)
        {
            PointsEarned = 3;
            return;
        }

        var predictedTrend = Math.Sign(LocalGoals - VisitorGoals);
        var realTrend = Math.Sign(realLocalGoals - realVisitorGoals);

        PointsEarned = predictedTrend == realTrend ? 1 : 0;
    }
}
