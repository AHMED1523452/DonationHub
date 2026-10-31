
using System.Security.Cryptography.X509Certificates;

namespace DonationHub.Application.Features.Auth.DTOs
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
