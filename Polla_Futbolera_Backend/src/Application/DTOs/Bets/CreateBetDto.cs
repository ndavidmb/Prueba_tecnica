namespace Application.DTOs.Bets;

public record CreateBetDto(int MatchId, int LocalGoals, int VisitorGoals);
