
using DonationHub.Application.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationHub.Domain.Entities
{
    public class PaymentAttempt : BaseEntity,ISoftDeletable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string StripeCheckoutSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string? FailureCode { get; set; }

        public Guid DonationId { get; set; }
        [ForeignKey("DonationId")]
        public Donation? Donation { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
