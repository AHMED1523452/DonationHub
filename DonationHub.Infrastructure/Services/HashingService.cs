
using DonationHub.Application.Interfaces;

namespace DonationHub.Infrastructure.Services
{
    public class HashingService : IHashingService
    {
        public async Task<string> HashPasswordAsync(string password, CancellationToken cancellationToken = default)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<string> HashTokenOrTextAsync(string Text , CancellationToken cancellationToken = default)
        {
            return BCrypt.Net.BCrypt.HashString(Text);
        }
        public async Task<bool> VerifyAsync(string hashed, string value, CancellationToken cancellationToken = default)
        {
            return BCrypt.Net.BCrypt.Verify(value, hashed);
        }
    }
}
