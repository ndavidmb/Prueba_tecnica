using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(m => m.LocalTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(m => m.VisitorTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Match_DifferentTeams",
            "\"LocalTeamId\" <> \"VisitorTeamId\""));
    }
}
