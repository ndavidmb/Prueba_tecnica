using Application.DTOs.Auth;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Services;

namespace Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        if (await userRepository.ExistsByEmailAsync(dto.Email))
            throw new DomainException("Ya existe un usuario con ese email.");

        var user = User.Create(dto.Name, dto.Email, dto.Password, passwordHasher);
        await userRepository.AddAsync(user);

        return new AuthResponseDto(
            jwtTokenGenerator.GenerateToken(user),
            user.Email,
            user.Role,
            user.Name);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email)
            ?? throw new DomainException("Credenciales inválidas.");

        if (!passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new DomainException("Credenciales inválidas.");

        return new AuthResponseDto(
            jwtTokenGenerator.GenerateToken(user),
            user.Email,
            user.Role,
            user.Name);
    }
}
