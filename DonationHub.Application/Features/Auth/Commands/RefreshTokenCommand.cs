
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands
{
    public sealed record RefreshTokenCommand(RefreshTokenRequestDTO requestDTO) : IRequest<GenericResult<RefreshTokenRequestDTO>>;
}
