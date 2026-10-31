
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationHub.Domain.Entities
{
    public class Donation : BaseEntity,ISoftDeletable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Money Amount { get; private set;  }

        //.There is a validation for this property for if the status is completed or succeeded 
        //. that's meaning can't be transfered or be pending (but will be done in handler after get the existed record)
        public string Status { get; private set; }
        public DateTime? CompleteAt { get; set; }
        public string? FailureReason { get; set; }
        public decimal? ReceiptNumber { get; set; }
        public string? ReceiptFileName { get; set; }
        public Guid CampaignId { get; set; }
        [ForeignKey("CampaignId")]
        public Campaign? Campaign { get; set; }
        public bool IsDeleted { get ; set; }
        public DateTime? DeletedAt { get ; set ; }

        private Donation() {}
        public Donation(decimal donationAmount, CurrencyValues currency)
        {
            if (donationAmount is < 1.00m or > 10000.00m)
                throw new ArgumentException("Donation amount must be between 1.00 and 10,000.00.");

            Amount = new Money(donationAmount, currency);
            Status = DonationStatus.Pending.ToString();
        }

    }
}
