
namespace DonationHub.Domain.Entities
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByIP { get; set; }
        public bool IsArchieved { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
