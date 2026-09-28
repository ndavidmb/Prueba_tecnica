using Application.DTOs.Common;
using Application.DTOs.Leaderboard;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/leaderboard")]
[Authorize]
public class LeaderboardController(ILeaderboardService leaderboardService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserLeaderboardDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeaderboard()
    {
        var result = await leaderboardService.GetLeaderboardAsync();
        return Ok(result);
    }

    [HttpGet("users/{userId}/history")]
    [ProducesResponseType(typeof(UserHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserHistory(int userId)
    {
        try
        {
            var result = await leaderboardService.GetUserHistoryAsync(userId);
            return Ok(result);
        }
        catch (DomainException ex)
        {
            return NotFound(new ErrorResponseDto(ex.Message));
        }
    }
}
