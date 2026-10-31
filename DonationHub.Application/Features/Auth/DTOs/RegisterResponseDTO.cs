namespace DonationHub.Application.Features.Auth.DTOs
{
    public class RegisterResponseDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}