
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;

namespace DonationHub.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<GenericResult<RefreshTokenRequestDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken);
    }
}
