using Domain.Enums;

namespace Application.DTOs.Matches;

public record MatchListItemDto(
    int Id,
    TeamDto LocalTeam,
    TeamDto VisitorTeam,
    int? LocalGoals,
    int? VisitorGoals,
    MatchStatus Status);
