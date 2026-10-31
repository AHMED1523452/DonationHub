using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Category.Commands
{
    public sealed record UpdateCategoryCommand(Guid Id, UpdateCategoryRequestDTO requestDTO)
        : IRequest<GenericResult<CategoryResponseDTO>>;
}
