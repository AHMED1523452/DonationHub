using DonationHub.Application.Features.Category.Commands;
using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using FluentValidation;
using MediatR;
using CampaignEntity = DonationHub.Domain.Entities.Campaign;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.Application.Features.Category.Handlers
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, GenericResult<CategoryResponseDTO>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly TimeProvider timeProvider;

        public UpdateCategoryHandler(IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            this.unitOfWork = unitOfWork;
            this.timeProvider = timeProvider;
        }

        public async Task<GenericResult<CategoryResponseDTO>> Handle(UpdateCategoryCommand request,
                                                                    CancellationToken cancellationToken)
        {
            var result = new GenericResult<CategoryResponseDTO>();
            var repository = unitOfWork.Repository<CategoryEntity>();

            var category = await repository.GetTrackedAsync(
                item => item.Id == request.Id,
                ignoreQueryFilters: true,
                cancellationToken: cancellationToken);

            if (category is null)
                return Failure(result, "Category not found.", 404);

            var campaignsCount = await unitOfWork.Repository<CampaignEntity>()
                                                .CountAsync(campaign => campaign.CategoryId == category.Id,
                                                            cancellationToken);
            if (campaignsCount > 0 && request.requestDTO.IsActive is false)
                return Failure(result,
                               $"This category is linked to {campaignsCount} campaign(s) and cannot be deactivated.",
                               409);

            var nameAr = request.requestDTO.NameAr.Trim();
            var nameEn = request.requestDTO.NameEn.Trim();

            var duplicate = await repository.GetByIdAsyncWithoutTracking(
                item => item.Id != request.Id && (item.NameAr == nameAr || item.NameEn == nameEn),
                cancellationToken);

            if (duplicate is not null)
                return Failure(result, "A category with the same Arabic or English name already exists.", 409);

            category.NameAr = nameAr;
            category.NameEn = nameEn;
            category.DescriptionAr = request.requestDTO.DescriptionAr.Trim();
            category.DescriptionEn = request.requestDTO.DescriptionEn.Trim();
            category.IsActive = request.requestDTO.IsActive;
            category.UpdatedAt = timeProvider.GetUtcNow().UtcDateTime;

            await unitOfWork.SavechangesAsync(cancellationToken);

            result.IsSuccess = true;
            result.Message = "Category updated successfully.";
            result.Data = new CategoryResponseDTO
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn,
                IsActive = category.IsActive,
                CampaignsCount = campaignsCount,
                CreatedAt = category.CreatedAt
            };

            return result;
        }

        private static GenericResult<CategoryResponseDTO> Failure(GenericResult<CategoryResponseDTO> result,
                                                                  string message,
                                                                  int statusCode)
        {
            result.IsSuccess = false;
            result.Message = message;
            result.StatusCode = statusCode;
            return result;
        }
    }
}
