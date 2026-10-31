using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Features.Category.Queries;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using FluentValidation;
using MediatR;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.Application.Features.Category.Handlers
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, PaginatedResult<CategoryResponseDTO>>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetCategoriesHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<CategoryResponseDTO>> Handle(GetCategoriesQuery request,
                                                                       CancellationToken cancellationToken)
        {
            var repository = unitOfWork.Repository<CategoryEntity>();

            var (items, totalCount) = await repository.GetPagedAsync<CategoryResponseDTO>(
                page: request.Page,
                pageSize: request.PageSize,
                predicate: null,
                selector: category => new CategoryResponseDTO
                {
                    Id = category.Id,
                    NameAr = category.NameAr,
                    NameEn = category.NameEn,
                    DescriptionAr = category.DescriptionAr,
                    DescriptionEn = category.DescriptionEn,
                    IsActive = category.IsActive,
                    CampaignsCount = category.campaigns.Count,
                    CreatedAt = category.CreatedAt
                },
                orderBy: nameof(CategoryEntity.NameEn),
                orderByDescending: false,
                searchTerm: request.SearchTerm,
                cancellationToken: cancellationToken);

            return new PaginatedResult<CategoryResponseDTO>
            {
                IsSuccess = true,
                Message = "Categories retrieved successfully.",
                Items = items,
                Pagination = new PaginationMetadata
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = request.PageSize > 0
                                 ? (int)Math.Ceiling(totalCount / (double)request.PageSize)
                                 : 0
                }
            };
        }
    }

}
