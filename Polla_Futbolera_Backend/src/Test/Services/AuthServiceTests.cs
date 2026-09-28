using Application.DTOs.Auth;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Services;
using Moq;

namespace Test.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepository.Object, _passwordHasher.Object, _jwtTokenGenerator.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_CreatesUserAndReturnsToken()
    {
        var dto = new RegisterUserDto("Jane Doe", "jane@test.com", "password123");
        _userRepository.Setup(r => r.ExistsByEmailAsync(dto.Email, default)).ReturnsAsync(false);
        _passwordHasher.Setup(h => h.HashPassword(dto.Password)).Returns("hashed-password");
        _jwtTokenGenerator.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("jwt-token");

        var result = await _sut.RegisterAsync(dto);

        Assert.Equal("jwt-token", result.Token);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal("Player", result.Role);
        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsDomainExceptionAndDoesNotPersist()
    {
        var dto = new RegisterUserDto("Jane Doe", "jane@test.com", "password123");
        _userRepository.Setup(r => r.ExistsByEmailAsync(dto.Email, default)).ReturnsAsync(true);

        await Assert.ThrowsAsync<DomainException>(() => _sut.RegisterAsync(dto));

        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var dto = new LoginDto("jane@test.com", "password123");
        var user = User.Create("Jane Doe", dto.Email, dto.Password, _passwordHasher.Object);
        _userRepository.Setup(r => r.GetByEmailAsync(dto.Email, default)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.VerifyPassword(dto.Password, user.PasswordHash)).Returns(true);
        _jwtTokenGenerator.Setup(j => j.GenerateToken(user)).Returns("jwt-token");

        var result = await _sut.LoginAsync(dto);

        Assert.Equal("jwt-token", result.Token);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownEmail_ThrowsDomainException()
    {
        var dto = new LoginDto("missing@test.com", "password123");
        _userRepository.Setup(r => r.GetByEmailAsync(dto.Email, default)).ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _sut.LoginAsync(dto));

        Assert.Equal("Credenciales inválidas.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsDomainException()
    {
        var dto = new LoginDto("jane@test.com", "wrong-password");
        var user = User.Create("Jane Doe", dto.Email, "correct-password", _passwordHasher.Object);
        _userRepository.Setup(r => r.GetByEmailAsync(dto.Email, default)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.VerifyPassword(dto.Password, user.PasswordHash)).Returns(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _sut.LoginAsync(dto));

        Assert.Equal("Credenciales inválidas.", exception.Message);
    }
}
