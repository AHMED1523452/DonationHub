using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        //. don't forget this query 
        builder.HasQueryFilter(user => !user.IsRevoked);
        builder.Property(user => user.FullName).HasMaxLength(200).IsRequired();

        builder.Property(user => user.preferredLanguage).HasConversion<string>()
                                                        .HasDefaultValue(PreferredLanguage.en)
                                                        .HasMaxLength(2)
                                                        .IsRequired();

        builder.Property(user => user.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(user => user.IsRevoked).HasDefaultValue(false).IsRequired();
        builder.Property(user => user.RevokedAt);
        builder.Property(user => user.CreatedAt).IsRequired();

    }
}
