using Application.DTOs.Bets;

namespace Application.Services;

public interface IBetService
{
    Task<BetResultDto> PlaceBetAsync(int userId, CreateBetDto dto);
}
