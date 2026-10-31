
using DonationHub.Application.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationHub.Domain.Entities
{
    public class Category : BaseEntity, ISoftDeletable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public bool IsDeleted { get ; set ; }
        public DateTime? DeletedAt { get; set ; }

        public ICollection<Campaign> campaigns = new List<Campaign>();
    }
}
