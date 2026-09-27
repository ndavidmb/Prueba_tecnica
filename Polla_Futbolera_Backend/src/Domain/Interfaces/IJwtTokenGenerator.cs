using Domain.Entities;

namespace Domain.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
