
using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Handlers
{
    public class CreateAdminOrStaffHandler
        : IRequestHandler<CreateAdminOrStaffCommand, GenericResult<CreateAdminOrStaffResponseDTO>>
    {
        private readonly ICreateAdminOrStaffService createAdminOrStaffService;

        public CreateAdminOrStaffHandler(ICreateAdminOrStaffService createAdminOrStaffService)
        {
            this.createAdminOrStaffService = createAdminOrStaffService;
        }

        public Task<GenericResult<CreateAdminOrStaffResponseDTO>> Handle(
            CreateAdminOrStaffCommand request,
            CancellationToken cancellationToken)
        {
            return createAdminOrStaffService.CreateAsync(request, cancellationToken);
        }
    }
}
