namespace Application.DTOs.Leaderboard;

public record UserBetHistoryItemDto(
    int BetId,
    int MatchId,
    string LocalTeamName,
    string VisitorTeamName,
    int PredictedLocalGoals,
    int PredictedVisitorGoals,
    int? RealLocalGoals,
    int? RealVisitorGoals,
    int PointsEarned);
