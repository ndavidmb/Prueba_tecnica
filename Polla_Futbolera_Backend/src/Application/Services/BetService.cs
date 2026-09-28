using Application.DTOs.Bets;
using Application.DTOs.Matches;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class BetService(
    IBetRepository betRepository,
    IMatchRepository matchRepository,
    ITeamRepository teamRepository) : IBetService
{
    public async Task<BetResultDto> PlaceBetAsync(int userId, CreateBetDto dto)
    {
        var match = await matchRepository.GetByIdAsync(dto.MatchId)
            ?? throw new DomainException("El partido no existe.");

        if (match.Status != MatchStatus.UpcomingMatch)
            throw new DomainException("No se pueden realizar apuestas en un partido que no esté pendiente (UpcomingMatch).");

        if (await betRepository.ExistsByUserAndMatchAsync(userId, dto.MatchId))
            throw new DomainException("Ya tienes una apuesta registrada para este partido.");

        var bet = Bet.Create(userId, dto.MatchId, dto.LocalGoals, dto.VisitorGoals);
        await betRepository.AddAsync(bet);

        return new BetResultDto(
            bet.Id,
            bet.UserId,
            bet.MatchId,
            bet.LocalGoals,
            bet.VisitorGoals,
            RealLocalGoals: null,
            RealVisitorGoals: null,
            PointsEarned: 0,
            IsExactMatch: false,
            IsTrendMatch: false);
    }

    public async Task<IEnumerable<UserBetDto>> GetUserBetsAsync(int userId)
    {
        var bets = await betRepository.GetByUserIdAsync(userId);
        var matches = await matchRepository.GetAllAsync();
        var matchesById = matches.ToDictionary(match => match.Id);
        var teams = await teamRepository.GetAllAsync();
        var teamsById = teams.ToDictionary(team => team.Id);

        return bets
            .OrderByDescending(bet => bet.MatchId)
            .Select(bet =>
            {
                var match = matchesById[bet.MatchId];
                return new UserBetDto(
                    bet.Id,
                    bet.MatchId,
                    ToTeamDto(teamsById[match.LocalTeamId]),
                    ToTeamDto(teamsById[match.VisitorTeamId]),
                    bet.LocalGoals,
                    bet.VisitorGoals,
                    match.LocalGoals,
                    match.VisitorGoals,
                    match.Status,
                    bet.PointsEarned);
            });
    }

    private static TeamDto ToTeamDto(Team team) => new(team.Id, team.Name);
}
