using Application.DTOs.Common;
using Application.DTOs.Matches;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/admin/matches")]
[Authorize(Roles = "Admin")]
public class AdminMatchesController(IMatchService matchService) : ControllerBase
{
    [HttpPut("{id}/result")]
    [ProducesResponseType(typeof(MatchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateResult(int id, UpdateMatchResultDto dto)
    {
        try
        {
            var result = await matchService.UpdateResultAsync(id, dto);
            return Ok(result);
        }
        catch (DomainException ex)
        {
            return BadRequest(new ErrorResponseDto(ex.Message));
        }
    }
}
