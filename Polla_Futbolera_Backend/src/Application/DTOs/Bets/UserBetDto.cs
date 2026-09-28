using Application.DTOs.Matches;
using Domain.Enums;

namespace Application.DTOs.Bets;

public record UserBetDto(
    int BetId,
    int MatchId,
    TeamDto LocalTeam,
    TeamDto VisitorTeam,
    int PredictedLocalGoals,
    int PredictedVisitorGoals,
    int? RealLocalGoals,
    int? RealVisitorGoals,
    MatchStatus MatchStatus,
    int PointsEarned);
