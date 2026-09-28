using Domain.Entities;
using Domain.Enums;
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

        builder.HasData(
            new { Id = 8, LocalTeamId = 5, VisitorTeamId = 6, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 9, LocalTeamId = 7, VisitorTeamId = 8, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 10, LocalTeamId = 5, VisitorTeamId = 7, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 11, LocalTeamId = 6, VisitorTeamId = 8, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 12, LocalTeamId = 8, VisitorTeamId = 5, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 13, LocalTeamId = 6, VisitorTeamId = 7, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 14, LocalTeamId = 1, VisitorTeamId = 2, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 15, LocalTeamId = 3, VisitorTeamId = 4, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 16, LocalTeamId = 1, VisitorTeamId = 3, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 17, LocalTeamId = 2, VisitorTeamId = 4, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 18, LocalTeamId = 4, VisitorTeamId = 1, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch },
            new { Id = 19, LocalTeamId = 2, VisitorTeamId = 3, LocalGoals = (int?)null, VisitorGoals = (int?)null, Status = MatchStatus.UpcomingMatch }
        );
    }
}
