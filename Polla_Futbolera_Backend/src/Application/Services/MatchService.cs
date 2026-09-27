using Application.DTOs.Matches;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class MatchService(IMatchRepository matchRepository) : IMatchService
{
    public async Task<MatchResponseDto> UpdateResultAsync(int matchId, UpdateMatchResultDto dto)
    {
        var match = await matchRepository.GetByIdAsync(matchId)
            ?? throw new DomainException("El partido no existe.");

        match.UpdateResult(dto.LocalGoals, dto.VisitorGoals);
        await matchRepository.UpdateAsync(match);

        return new MatchResponseDto(
            match.Id,
            match.LocalTeamId,
            match.VisitorTeamId,
            match.LocalGoals,
            match.VisitorGoals,
            match.Status);
    }
}
