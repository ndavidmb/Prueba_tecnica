namespace Application.DTOs.Leaderboard;

public record UserHistoryDto(
    int UserId,
    string UserName,
    int TotalPoints,
    List<UserBetHistoryItemDto> Bets);
