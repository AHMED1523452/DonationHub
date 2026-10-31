
using DonationHub.Domain.Enums;

namespace DonationHub.Application.Features.Auth.DTOs
{
    public class RegisterRequestDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public PreferredLanguage? PreferredLanguage { get; set; } = Domain.Enums.PreferredLanguage.en;

    }
}
