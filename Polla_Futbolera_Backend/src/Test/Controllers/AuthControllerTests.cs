using Application.DTOs.Auth;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;

namespace Test.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService = new();
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _sut = new AuthController(_authService.Object);
    }

    [Fact]
    public async Task Register_WithNewUser_ReturnsCreatedWithResponse()
    {
        var dto = new RegisterUserDto("Jane Doe", "jane@test.com", "password123");
        var response = new AuthResponseDto("jwt-token", dto.Email, "Player", dto.Name);
        _authService.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(response);

        var result = await _sut.Register(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task Register_WhenEmailAlreadyExists_ReturnsConflict()
    {
        var dto = new RegisterUserDto("Jane Doe", "jane@test.com", "password123");
        _authService.Setup(s => s.RegisterAsync(dto))
            .ThrowsAsync(new DomainException("Ya existe un usuario con ese email."));

        var result = await _sut.Register(dto);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithResponse()
    {
        var dto = new LoginDto("jane@test.com", "password123");
        var response = new AuthResponseDto("jwt-token", dto.Email, "Player", "Jane Doe");
        _authService.Setup(s => s.LoginAsync(dto)).ReturnsAsync(response);

        var result = await _sut.Login(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var dto = new LoginDto("jane@test.com", "wrong-password");
        _authService.Setup(s => s.LoginAsync(dto)).ThrowsAsync(new DomainException("Credenciales inválidas."));

        var result = await _sut.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
