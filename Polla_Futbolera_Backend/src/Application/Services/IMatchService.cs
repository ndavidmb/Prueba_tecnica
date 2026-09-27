using Application.DTOs.Matches;

namespace Application.Services;

public interface IMatchService
{
    Task<MatchResponseDto> UpdateResultAsync(int matchId, UpdateMatchResultDto dto);
}
