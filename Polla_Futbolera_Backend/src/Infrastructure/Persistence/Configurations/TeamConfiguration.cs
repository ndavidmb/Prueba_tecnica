using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasData(
            new { Id = 1, Name = "Real Madrid" },
            new { Id = 2, Name = "Bayern München" },
            new { Id = 3, Name = "Paris Saint-Germain" },
            new { Id = 4, Name = "Inter de Milán" },
            new { Id = 5, Name = "Manchester City" },
            new { Id = 6, Name = "FC Barcelona" },
            new { Id = 7, Name = "Borussia Dortmund" },
            new { Id = 8, Name = "Arsenal" }
        );
    }
}
