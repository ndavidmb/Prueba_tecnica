using Application.DTOs.Auth;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        try
        {
            var response = await authService.RegisterAsync(dto);
            return CreatedAtAction(nameof(Register), response);
        }
        catch (DomainException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var response = await authService.LoginAsync(dto);
            return Ok(response);
        }
        catch (DomainException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
