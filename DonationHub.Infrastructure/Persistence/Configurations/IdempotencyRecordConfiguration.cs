using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");
        builder.HasKey(record => record.Key);
        builder.Property(record => record.UserId).HasMaxLength(36).IsRequired();
        builder.Property(record => record.RequestHash).HasMaxLength(128).IsRequired();
        builder.Property(record => record.ResponseBody).HasMaxLength(4000).IsRequired();
        builder.Property(record => record.ResponseStatusCode).HasMaxLength(3).IsRequired();
        builder.HasIndex(record => new { record.UserId, record.RequestHash }).IsUnique();
    }
}
