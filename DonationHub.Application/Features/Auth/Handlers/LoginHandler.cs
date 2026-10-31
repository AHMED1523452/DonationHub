using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, GenericResult<LoginResponseDTO>>
    {
        private readonly ILoginService service;

        public LoginHandler(ILoginService service)
        {
            this.service = service;
        }
        public async Task<GenericResult<LoginResponseDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
            => await service.LoginAsync(request.requestDTO, cancellationToken);
    }

}
