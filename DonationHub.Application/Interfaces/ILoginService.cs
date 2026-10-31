
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;

namespace DonationHub.Application.Interfaces
{
    public interface ILoginService
    {
        Task<GenericResult<LoginResponseDTO>> LoginAsync(
            LoginRequestDTO request,
            CancellationToken cancellationToken = default);

        Task<GenericResult<string>> LogoutAsync(CancellationToken cancellationToken = default);
    }
}
