using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Features.Category.Queries;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using MediatR;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.Application.Features.Category.Handlers
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, GenericResult<CategoryResponseDTO>>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetCategoryByIdHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<GenericResult<CategoryResponseDTO>> Handle(GetCategoryByIdQuery request,
                                                                     CancellationToken cancellationToken)
        {
            var result = new GenericResult<CategoryResponseDTO>();
            var repository = unitOfWork.Repository<CategoryEntity>();

            var category = await repository.GetTrackedAsync(
                item => item.Id == request.Id,
                false,
                cancellationToken,
                item => item.campaigns);

            if (category is null)
            {
                result.IsSuccess = false;
                result.Message = "Category not found.";
                result.StatusCode = 404;
                return result;
            }

            result.IsSuccess = true;
            result.Message = "Category retrieved successfully.";
            result.Data = new CategoryResponseDTO
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn,
                IsActive = category.IsActive,
                CampaignsCount = category.campaigns.Count,
                CreatedAt = category.CreatedAt
            };

            return result;
        }
    }
}
