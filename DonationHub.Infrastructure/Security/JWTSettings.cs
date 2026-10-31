
namespace DonationHub.Infrastructure.Security
{
    public class JWTSettings
    {
        public const string SectionName = "JWTSettings";

        public string SecretKEY { get; init; } = null!;
        public int ExpiryMinutes { get; init; }
        public int RefreshTokenExpiryDays { get; init; }
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
    }
}
