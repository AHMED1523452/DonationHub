
using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Handlers
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, GenericResult<RegisterResponseDTO>>
    {
        private readonly IRegisterService registerService;

        public RegisterHandler(IRegisterService registerService)
        {
            this.registerService = registerService;
        }

        public Task<GenericResult<RegisterResponseDTO>> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken)
        {
            return registerService.RegisterDonorAsync(request.requestDTO, cancellationToken);
        }
    }
}
