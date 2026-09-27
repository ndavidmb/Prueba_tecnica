using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BetConfiguration : IEntityTypeConfiguration<Bet>
{
    public void Configure(EntityTypeBuilder<Bet> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasIndex(b => new { b.UserId, b.MatchId }).IsUnique();

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Bet_PointsEarned",
            "\"PointsEarned\" IN (0, 1, 3)"));

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Match>()
            .WithMany()
            .HasForeignKey(b => b.MatchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
