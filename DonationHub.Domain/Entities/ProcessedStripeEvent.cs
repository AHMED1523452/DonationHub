
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationHub.Domain.Entities
{
    public class ProcessedStripeEvent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventId { get; set; }
        public string Type { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
