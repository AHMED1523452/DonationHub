
using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Handlers
{
    public class RefrehTokenHandler : IRequestHandler<RefreshTokenCommand, GenericResult<RefreshTokenRequestDTO>>
    {
        private readonly IRefreshTokenService service;

        public RefrehTokenHandler(IRefreshTokenService service)
        {
            this.service = service;
        }
        public async Task<GenericResult<RefreshTokenRequestDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
            => await service.RefreshTokenAsync(request.requestDTO, cancellationToken);
    }
}
