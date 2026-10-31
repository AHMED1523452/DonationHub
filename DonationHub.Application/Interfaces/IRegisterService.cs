
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;

namespace DonationHub.Application.Interfaces
{
    public interface IRegisterService
    {
        Task<GenericResult<RegisterResponseDTO>> RegisterDonorAsync(
            RegisterRequestDTO request,
            CancellationToken cancellationToken = default);
    }
}
