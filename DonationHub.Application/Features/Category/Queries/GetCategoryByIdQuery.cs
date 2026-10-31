using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Category.Queries
{
    public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<GenericResult<CategoryResponseDTO>>;
}
