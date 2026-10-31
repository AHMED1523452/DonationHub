
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;
using DonationHub.Domain.Enums;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands
{
    public class CreateAdminOrStaffCommand : IRequest<GenericResult<CreateAdminOrStaffResponseDTO>>
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.Donor;
        public PreferredLanguage? PreferredLanguage { get; set; } = Domain.Enums.PreferredLanguage.en;
    }
}
