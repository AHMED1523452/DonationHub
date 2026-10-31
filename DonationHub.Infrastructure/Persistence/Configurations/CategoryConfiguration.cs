using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public sealed class CategoryConfiguration : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(category => category.Id);
        builder.Property(category => category.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(category => category.NameEn).HasMaxLength(100).IsRequired();
        builder.HasIndex(category => category.NameAr);
        builder.HasIndex(category => category.NameEn);

        builder.Property(category => category.DescriptionAr).HasMaxLength(1000);
        builder.Property(category => category.DescriptionEn).HasMaxLength(1000);

        builder.HasQueryFilter(category => !category.IsDeleted && 
                                            category.IsActive && 
                                           !category.IsArchieved);

        ConfigureBaseProperties(builder);
    }
}
