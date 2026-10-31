
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Features.Auth.Queries;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Handlers
{
    public class AuthMeHandler : IRequestHandler<AuthMeQuery, GenericResult<UserProfileResponseDTO>>
    {
        private readonly IAuthMeService service;

        public AuthMeHandler(IAuthMeService service)
        {
            this.service = service;
        }
        public async Task<GenericResult<UserProfileResponseDTO>> Handle(AuthMeQuery request, CancellationToken cancellationToken)
            => await service.Profile(cancellationToken);
    }
}
