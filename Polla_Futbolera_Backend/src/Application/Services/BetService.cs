using Application.DTOs.Bets;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class BetService(
    IBetRepository betRepository,
    IMatchRepository matchRepository) : IBetService
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
}
