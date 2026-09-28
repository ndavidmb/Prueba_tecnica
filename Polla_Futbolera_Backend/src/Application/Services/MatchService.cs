using Application.DTOs.Matches;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class MatchService(
    IMatchRepository matchRepository,
    IBetRepository betRepository,
    ITeamRepository teamRepository) : IMatchService
{
    public async Task<IEnumerable<MatchListItemDto>> GetAllAsync()
    {
        var matches = await matchRepository.GetAllAsync();
        var teams = await teamRepository.GetAllAsync();
        var teamsById = teams.ToDictionary(team => team.Id);

        return matches
            .OrderBy(match => match.Status)
            .ThenBy(match => match.Id)
            .Select(match => new MatchListItemDto(
                match.Id,
                ToTeamDto(teamsById[match.LocalTeamId]),
                ToTeamDto(teamsById[match.VisitorTeamId]),
                match.LocalGoals,
                match.VisitorGoals,
                match.Status));
    }

    public async Task<MatchResponseDto> UpdateResultAsync(int matchId, UpdateMatchResultDto dto)
    {
        var match = await matchRepository.GetByIdAsync(matchId)
            ?? throw new DomainException("El partido no existe.");

        match.UpdateResult(dto.LocalGoals, dto.VisitorGoals);
        await matchRepository.UpdateAsync(match);

        var bets = await betRepository.GetByMatchIdAsync(matchId);
        var betsList = bets.ToList();
        foreach (var bet in betsList)
            bet.CalculateAndAssignPoints(dto.LocalGoals, dto.VisitorGoals);

        if (betsList.Count > 0)
            await betRepository.UpdateRangeAsync(betsList);

        return new MatchResponseDto(
            match.Id,
            match.LocalTeamId,
            match.VisitorTeamId,
            match.LocalGoals,
            match.VisitorGoals,
            match.Status);
    }

    private static TeamDto ToTeamDto(Team team) => new(team.Id, team.Name);
}
