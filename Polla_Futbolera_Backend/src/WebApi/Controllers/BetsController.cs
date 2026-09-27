using System.Security.Claims;
using Application.DTOs.Bets;
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
    [HttpPost]
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
            return Conflict(new { message = ex.Message });
        }
    }
}
