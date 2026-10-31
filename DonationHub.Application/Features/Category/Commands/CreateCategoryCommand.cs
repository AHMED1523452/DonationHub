using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Category.Commands
{
    public sealed record CreateCategoryCommand(CreateCategoryRequestDTO requestDTO)
        : IRequest<GenericResult<CategoryResponseDTO>>;
}
