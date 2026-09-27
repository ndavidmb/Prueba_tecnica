using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Match
{
    public int Id { get; private set; }
    public int LocalTeamId { get; private set; }
    public int VisitorTeamId { get; private set; }
    public int? LocalGoals { get; private set; }
    public int? VisitorGoals { get; private set; }
    public MatchStatus Status { get; private set; }

    private Match() { }

    public static Match Create(int localTeamId, int visitorTeamId)
    {
        if (localTeamId == visitorTeamId)
            throw new DomainException("Un equipo no puede jugar contra sí mismo.");

        return new Match
        {
            LocalTeamId = localTeamId,
            VisitorTeamId = visitorTeamId,
            Status = MatchStatus.UpcomingMatch
        };
    }

    public void UpdateResult(int localGoals, int visitorGoals)
    {
        if (localGoals < 0 || visitorGoals < 0)
            throw new DomainException("Los goles no pueden ser negativos.");

        LocalGoals = localGoals;
        VisitorGoals = visitorGoals;
        Status = MatchStatus.FullTime;
    }
}
