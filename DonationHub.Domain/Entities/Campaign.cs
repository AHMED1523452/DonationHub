
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationHub.Domain.Entities
{
    public class Campaign : BaseEntity, ISoftDeletable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public decimal GoalAmount { get; private set; }
        public decimal RaisedAmount { get; set; }
        public DateTime EndDate { get; private set; }
        public string? BannerName { get; set; }
        public string Status { get; private set; }
        public byte[] RowVersion { get; private set; } //. will be created automatic from SQL server
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public bool IsDeleted { get ; set ; }
        public DateTime? DeletedAt { get ; set ; }

        public ICollection<Donation> Donations = new List<Donation>();

        private Campaign() { }

        public Campaign(decimal goalAmount,
                        DateTime endDate,
                        DateTime utcNow)
        {
            GoalAmount = goalAmount <= 0 ?
                    throw new ArgumentException("Amount must be greater than 0")
                    : goalAmount;

            EndDate = endDate <= utcNow ?
                    throw new ArgumentException("Invalid End date. ")
                    : endDate;

            Status = CampaignStatus.Active.ToString();

        }

        public void ChangeEndDate(decimal goalAmount,DateTime endDate)
        {
            GoalAmount = goalAmount <= 0 ?
                    throw new ArgumentException("Amount must be greater than 0")
                    : goalAmount;

            EndDate = endDate <= DateTime.UtcNow ? throw new ArgumentException("Invaid end date.")
                                   : endDate;
        }
    }
}
