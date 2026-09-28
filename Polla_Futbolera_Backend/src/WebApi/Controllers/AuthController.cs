using Application.DTOs.Auth;
using Application.DTOs.Common;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        try
        {
            var response = await authService.RegisterAsync(dto);
            return CreatedAtAction(nameof(Register), response);
        }
        catch (DomainException ex)
        {
            return Conflict(new ErrorResponseDto(ex.Message));
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var response = await authService.LoginAsync(dto);
            return Ok(response);
        }
        catch (DomainException ex)
        {
            return Unauthorized(new ErrorResponseDto(ex.Message));
        }
    }
}
