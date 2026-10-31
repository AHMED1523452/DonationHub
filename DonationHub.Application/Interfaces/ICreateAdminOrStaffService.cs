using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;

namespace DonationHub.Application.Interfaces
{
    public interface ICreateAdminOrStaffService
    {
        Task<GenericResult<CreateAdminOrStaffResponseDTO>> CreateAsync(
            CreateAdminOrStaffCommand command,
            CancellationToken cancellationToken = default);
    }
}
