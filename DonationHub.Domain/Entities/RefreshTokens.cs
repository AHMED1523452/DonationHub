
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationHub.Domain.Entities
{
    public class RefreshTokens
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string TokenHash { get; set; }
        public string? ReplacedByTokenId { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
