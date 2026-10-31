using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Configurations;

public sealed class PaymentAttemptConfiguration : BaseEntityConfiguration<PaymentAttempt>
{
    public override void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.ToTable("PaymentAttempts");
        builder.HasKey(attempt => attempt.Id);
        builder.Property(attempt => attempt.StripeCheckoutSessionId).HasMaxLength(255).IsRequired();
        builder.Property(attempt => attempt.StripePaymentIntentId).HasMaxLength(255);
        builder.Property(attempt => attempt.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(attempt => attempt.Status).HasMaxLength(50).IsRequired();
        builder.Property(attempt => attempt.FailureCode).HasMaxLength(100);
        builder.HasIndex(attempt => attempt.DonationId);
        builder.HasIndex(attempt => attempt.StripeCheckoutSessionId).IsUnique();
        builder.HasQueryFilter(attempt => !attempt.IsDeleted && attempt.IsActive && !attempt.IsArchieved);

        builder.HasOne(attempt => attempt.Donation)
            .WithMany()
            .HasForeignKey(attempt => attempt.DonationId)
            .OnDelete(DeleteBehavior.Restrict);

        ConfigureBaseProperties(builder);
    }
}
