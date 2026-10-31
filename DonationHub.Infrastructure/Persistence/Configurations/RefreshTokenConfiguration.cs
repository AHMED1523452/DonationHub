using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokens>
{
    public void Configure(EntityTypeBuilder<RefreshTokens> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(token => token.Id);
        builder.Property(token => token.UserId).HasMaxLength(36).IsRequired();
        builder.Property(token => token.TokenHash).HasMaxLength(512).IsRequired();
        builder.Property(token => token.ReplacedByTokenId).HasMaxLength(36);
        builder.HasIndex(token => token.TokenHash).IsUnique();
        builder.Property(token => token.IsRevoked).HasDefaultValue(false).IsRequired();
        builder.HasIndex(token => token.UserId);
    }
}
