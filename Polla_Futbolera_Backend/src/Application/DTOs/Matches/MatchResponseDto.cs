using Domain.Enums;

namespace Application.DTOs.Matches;

public record MatchResponseDto(
    int Id,
    int LocalTeamId,
    int VisitorTeamId,
    int? LocalGoals,
    int? VisitorGoals,
    MatchStatus Status);
