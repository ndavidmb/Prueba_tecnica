using Domain.Services;

namespace Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string plainText) => BCrypt.Net.BCrypt.HashPassword(plainText);
    public bool VerifyPassword(string plainText, string hash) => BCrypt.Net.BCrypt.Verify(plainText, hash);
}
