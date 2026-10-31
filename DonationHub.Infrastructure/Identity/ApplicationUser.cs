using Microsoft.AspNetCore.Identity;

namespace DonationHub.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public string preferredLanguage { get; set; } = "en";
        public bool IsActive { get; set; } = true;
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
