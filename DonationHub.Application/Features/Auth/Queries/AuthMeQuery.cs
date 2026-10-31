
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Auth.Queries
{
    public sealed record AuthMeQuery : IRequest<GenericResult<UserProfileResponseDTO>>;
}
