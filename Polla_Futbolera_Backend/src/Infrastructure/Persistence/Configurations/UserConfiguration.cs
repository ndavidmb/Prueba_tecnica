using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    // Hash BCrypt de la contraseña semilla "Password123!" (solo para entornos de desarrollo).
    private const string SeedPasswordHash = "$2b$11$DaqubqB.PVWKKtzFqBknAuef4ywFvt5XO5EtRsM.8/4p/IpCud2Ca";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasColumnName("password");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasData(
            new { Id = 1, Name = "Juan Díaz", Email = "player1@player.com", PasswordHash = SeedPasswordHash, Role = "Player" },
            new { Id = 2, Name = "Luisa Perez", Email = "admin@admin.com", PasswordHash = SeedPasswordHash, Role = "Admin" },
            new { Id = 3, Name = "Mario Torres", Email = "player@player.com", PasswordHash = SeedPasswordHash, Role = "Player" }
        );
    }
}
