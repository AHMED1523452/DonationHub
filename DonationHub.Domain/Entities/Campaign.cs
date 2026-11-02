
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationHub.Domain.Entities
{
    public class Campaign : BaseEntity, ISoftDeletable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public string TitleAr { get; private set; }
        public string TitleEn { get; private set; }
        public string? DescriptionAr { get; private set; }
        public string? DescriptionEn { get; private set; }
        public Money GoalAmount { get; private set; }
        public decimal RaisedAmount { get; private set; }
        public DateTime EndDate { get; private set; }
        public string? BannerName { get; private set; }
        public string Status { get; private set; }
        public byte[] RowVersion { get; private set; } //. will be created automatic from SQL server
        public Guid CategoryId { get; private set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; private set; }
        public bool IsDeleted { get ; set ; }
        public DateTime? DeletedAt { get ; set ; }

        public ICollection<Donation> Donations = new List<Donation>();

        private Campaign() { }

        public Campaign(string titleAr,
                        string titleEn,
                        string? descriptionAr,
                        string? descriptionEn,
                        string? bannerName,
                        decimal goalAmount,
                        CurrencyValues currency,
                        DateTime endDate,
                        Guid categoryId,
                        DateTime utcNow)
        {
            TitleAr = string.IsNullOrWhiteSpace(titleAr)
                ? throw new ArgumentException("Title (AR) is required.", nameof(titleAr))
                : titleAr.Trim();

            TitleEn = string.IsNullOrWhiteSpace(titleEn)
                ? throw new ArgumentException("Title (EN) is required.", nameof(titleEn))
                : titleEn.Trim();

            CategoryId = categoryId == Guid.Empty
                ? throw new ArgumentException("Category is required.", nameof(categoryId))
                : categoryId;

            GoalAmount = new Money(goalAmount, currency);

            EndDate = endDate <= utcNow ?
                    throw new ArgumentException("Invalid End date. ")
                    : endDate;

            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
            BannerName = bannerName;
            RaisedAmount = 0m;
            Status = CampaignStatus.Active.ToString();
        }

        public void ChangeEndDate(decimal goalAmount, CurrencyValues currency, DateTime endDate)
        {
            GoalAmount = new Money(goalAmount, currency);

            EndDate = endDate <= DateTime.UtcNow ? throw new ArgumentException("Invaid end date.")
                                   : endDate;
        }
    }
}
