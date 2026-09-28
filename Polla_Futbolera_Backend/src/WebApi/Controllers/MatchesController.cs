using Application.DTOs.Matches;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/matches")]
[Authorize]
public class MatchesController(IMatchService matchService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MatchListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await matchService.GetAllAsync();
        return Ok(result);
    }
}
