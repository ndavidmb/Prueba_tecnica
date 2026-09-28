using Domain.Entities;
using Domain.Exceptions;
using Domain.Services;
using Moq;

namespace Test.Domain;

public class UserTests
{
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    [Fact]
    public void Create_WithValidData_HashesPasswordAndAssignsPlayerRole()
    {
        _passwordHasher.Setup(h => h.HashPassword("plain-password")).Returns("hashed-password");

        var user = User.Create("Jane Doe", "jane@test.com", "plain-password", _passwordHasher.Object);

        Assert.Equal("Jane Doe", user.Name);
        Assert.Equal("jane@test.com", user.Email);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.Equal("Player", user.Role);
    }

    [Theory]
    [InlineData("", "jane@test.com", "password")]
    [InlineData("   ", "jane@test.com", "password")]
    [InlineData("Jane Doe", "", "password")]
    [InlineData("Jane Doe", "jane@test.com", "")]
    public void Create_WithMissingRequiredField_ThrowsDomainException(string name, string email, string password)
    {
        Assert.Throws<DomainException>(() => User.Create(name, email, password, _passwordHasher.Object));
    }

    [Fact]
    public void UpdatePassword_WithValidPassword_UpdatesHash()
    {
        _passwordHasher.Setup(h => h.HashPassword("initial")).Returns("hash-1");
        _passwordHasher.Setup(h => h.HashPassword("updated")).Returns("hash-2");
        var user = User.Create("Jane Doe", "jane@test.com", "initial", _passwordHasher.Object);

        user.UpdatePassword("updated", _passwordHasher.Object);

        Assert.Equal("hash-2", user.PasswordHash);
    }

    [Fact]
    public void UpdatePassword_WithEmptyPassword_ThrowsDomainException()
    {
        _passwordHasher.Setup(h => h.HashPassword("initial")).Returns("hash-1");
        var user = User.Create("Jane Doe", "jane@test.com", "initial", _passwordHasher.Object);

        Assert.Throws<DomainException>(() => user.UpdatePassword("   ", _passwordHasher.Object));
    }
}
