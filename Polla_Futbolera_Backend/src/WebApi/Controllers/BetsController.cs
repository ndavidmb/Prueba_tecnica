using System.Security.Claims;
using Application.DTOs.Bets;
using Application.DTOs.Common;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/bets")]
[Authorize]
public class BetsController(IBetService betService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserBetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBets()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await betService.GetUserBetsAsync(userId);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BetResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PlaceBet(CreateBetDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var result = await betService.PlaceBetAsync(userId, dto);
            return CreatedAtAction(nameof(PlaceBet), result);
        }
        catch (DomainException ex)
        {
            return Conflict(new ErrorResponseDto(ex.Message));
        }
    }
}
