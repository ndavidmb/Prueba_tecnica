namespace Application.DTOs.Bets;

public record BetResultDto(
    int BetId,
    int UserId,
    int MatchId,
    int PredictedLocalGoals,
    int PredictedVisitorGoals,
    int? RealLocalGoals,
    int? RealVisitorGoals,
    int PointsEarned,
    bool IsExactMatch,
    bool IsTrendMatch);
