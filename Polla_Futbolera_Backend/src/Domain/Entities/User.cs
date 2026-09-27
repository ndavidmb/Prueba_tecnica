using Domain.Exceptions;
using Domain.Services;

namespace Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }

    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
        Role = string.Empty;
    }

    public static User Create(string name, string email, string plainPassword, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del usuario no puede ser vacío.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("El email no puede ser vacío.");
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new DomainException("La contraseña no puede ser vacía.");

        return new User
        {
            Name = name,
            Email = email,
            PasswordHash = hasher.HashPassword(plainPassword),
            Role = "Player"
        };
    }

    public void UpdatePassword(string newPlainPassword, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(newPlainPassword))
            throw new DomainException("La nueva contraseña no puede ser vacía.");

        PasswordHash = hasher.HashPassword(newPlainPassword);
    }
}
