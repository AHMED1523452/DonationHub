
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;

namespace DonationHub.Application.Interfaces
{
    public interface IAuthMeService
    {
        Task<GenericResult<UserProfileResponseDTO>> Profile(CancellationToken cancellationToken = default);
    }
}
