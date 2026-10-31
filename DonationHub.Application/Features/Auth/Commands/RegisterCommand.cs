using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands
{
    public sealed record RegisterCommand(RegisterRequestDTO requestDTO) : IRequest<GenericResult<RegisterResponseDTO>>;
}
