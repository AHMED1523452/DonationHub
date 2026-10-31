using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    //. the function that will be ovverrided in the another configuration classes
    public abstract void Configure(EntityTypeBuilder<TEntity> builder);

    protected static void ConfigureBaseProperties(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(entity => entity.CreatedAt).IsRequired();
        builder.Property(entity => entity.UpdatedAt).IsRequired();
        builder.Property(entity => entity.CreatedByUserId).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.CreatedByIP).HasMaxLength(45);
        builder.Property(entity => entity.IsActive).HasDefaultValue(true);
        builder.Property(entity => entity.IsArchieved).HasDefaultValue(false);
    }
}
