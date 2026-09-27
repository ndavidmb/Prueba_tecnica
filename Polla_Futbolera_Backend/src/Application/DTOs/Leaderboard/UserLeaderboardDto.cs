namespace Application.DTOs.Leaderboard;

public record UserLeaderboardDto(
    int UserId,
    string UserName,
    int TotalPoints,
    int TotalBets,
    int RankPosition);
