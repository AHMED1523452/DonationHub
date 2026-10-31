
namespace DonationHub.Application.Interfaces
{
    public interface IHashingService
    {
        Task<string> HashPasswordAsync(string password, CancellationToken cancellationToken = default);
        Task<string> HashTokenOrTextAsync(string Text, CancellationToken cancellationToken = default);
        Task<bool> VerifyAsync(string hashed, string value, CancellationToken cancellationToken = default);
    }
}
