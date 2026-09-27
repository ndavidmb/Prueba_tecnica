using Application.DTOs.Auth;

namespace Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
