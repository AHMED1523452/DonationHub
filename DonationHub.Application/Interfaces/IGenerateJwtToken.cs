namespace DonationHub.Application.Interfaces
{
    public interface IGenerateJwtToken
    {
        Task<string> GenerateAccessTokenAsync(string Id,
                                                           string Email,
                                                           string? UserName,
                                                           string FullName,
                                                           string? role, CancellationToken cancellationToken = default);

        Task<string> GenerateRefreshTokenAsync(string Id,
                                                         string Email,
                                                         string? UserName,
                                                         string FullName,
                                                         string? role, CancellationToken cancellationToken = default);
    }
}
