using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public sealed class ProcessedStripeEventConfiguration : IEntityTypeConfiguration<ProcessedStripeEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedStripeEvent> builder)
    {
        builder.ToTable("ProcessedStripeEvents");
        builder.HasKey(stripeEvent => stripeEvent.EventId);
        builder.Property(stripeEvent => stripeEvent.Type).HasMaxLength(100).IsRequired();
        builder.HasIndex(stripeEvent => stripeEvent.ProcessedAt);
    }
}
