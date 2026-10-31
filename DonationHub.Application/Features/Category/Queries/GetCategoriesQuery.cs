using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Patterns;
using MediatR;

namespace DonationHub.Application.Features.Category.Queries
{
    public sealed record GetCategoriesQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null)
        : IRequest<PaginatedResult<CategoryResponseDTO>>;
}
