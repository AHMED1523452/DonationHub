using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public sealed class CampaignConfiguration : BaseEntityConfiguration<Campaign>
{
    public override void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");
        builder.HasKey(campaign => campaign.Id);
        builder.Property(campaign => campaign.TitleAr).HasMaxLength(200).IsRequired();
        builder.Property(campaign => campaign.TitleEn).HasMaxLength(200).IsRequired();
        builder.Property(campaign => campaign.DescriptionAr).HasMaxLength(4000);
        builder.Property(campaign => campaign.DescriptionEn).HasMaxLength(4000);
        builder.Property(campaign => campaign.GoalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(campaign => campaign.RaisedAmount).HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(campaign => campaign.EndDate).IsRequired();
        builder.Property(campaign => campaign.BannerName).HasMaxLength(255);
        builder.Property(campaign => campaign.Status).HasMaxLength(50).IsRequired();

        builder.Property(campaign => campaign.RowVersion)
            .HasMaxLength(40)
            .IsConcurrencyToken();

        builder.Property(campign => campign.RowVersion)
            .IsRowVersion();

        builder.HasIndex(campaign => campaign.CategoryId);
        builder.HasQueryFilter(campaign => !campaign.IsDeleted && campaign.IsActive && !campaign.IsArchieved);

        builder.HasOne(campaign => campaign.Category)
            .WithMany(category => category.campaigns)
            .HasForeignKey(campaign => campaign.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        ConfigureBaseProperties(builder);
    }
}
