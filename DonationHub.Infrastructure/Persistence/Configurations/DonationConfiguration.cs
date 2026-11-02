using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public sealed class DonationConfiguration : BaseEntityConfiguration<Donation>
{
    public override void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToTable("Donations", table =>
            table.HasCheckConstraint("CK_Donations_Amount_Range", "[Amount] >= 1.00 AND [Amount] <= 10000.00"));

        builder.HasKey(donation => donation.Id);
        builder.OwnsOne(donation => donation.Amount, money =>
        {
            money.Property(value => value.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .HasDefaultValue(1.00m)
                .IsRequired();

            money.Property(value => value.currency)
                .HasColumnName("Currency")
                .HasConversion<string>()
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.Property(donation => donation.Status).HasMaxLength(50).IsRequired();
        builder.Property(donation => donation.FailureReason).HasMaxLength(500);

        builder.Property(donation => donation.ReceiptNumber).HasPrecision(20, 0);

        builder.Property(donation => donation.ReceiptFileName).HasMaxLength(255);
        builder.HasIndex(donation => donation.CampaignId);
        builder.HasQueryFilter(donation => !donation.IsDeleted && donation.IsActive && !donation.IsArchieved);

        builder.HasOne(donation => donation.Campaign)
            .WithMany(campaign => campaign.Donations)
            .HasForeignKey(donation => donation.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey("CreatedByUserId")
               .OnDelete(DeleteBehavior.Restrict);

        ConfigureBaseProperties(builder);
    }
}
