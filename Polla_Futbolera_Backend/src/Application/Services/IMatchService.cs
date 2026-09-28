using Application.DTOs.Matches;

namespace Application.Services;

public interface IMatchService
{
    Task<IEnumerable<MatchListItemDto>> GetAllAsync();
    Task<MatchResponseDto> UpdateResultAsync(int matchId, UpdateMatchResultDto dto);
}
