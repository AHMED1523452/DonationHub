using DonationHub.Application.Factories;
using DonationHub.Application.Features.Category.Commands;
using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using FluentValidation;
using MediatR;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.Application.Features.Category.Handlers
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, GenericResult<CategoryResponseDTO>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IModelFactory modelFactory;
        private readonly ICurrentUserService currentUserService;

        public CreateCategoryHandler(IUnitOfWork unitOfWork,
                                     IModelFactory modelFactory,
                                     ICurrentUserService currentUserService)
        {
            this.unitOfWork = unitOfWork;
            this.modelFactory = modelFactory;
            this.currentUserService = currentUserService;
        }

        public async Task<GenericResult<CategoryResponseDTO>> Handle(CreateCategoryCommand request,
                                                                    CancellationToken cancellationToken)
        {
            var result = new GenericResult<CategoryResponseDTO>();
            var repository = unitOfWork.Repository<CategoryEntity>();

            //. removing the spaces from start and end in text 
            var nameAr = request.requestDTO.NameAr.Trim();
            var nameEn = request.requestDTO.NameEn.Trim();

            //. check duplication for the name of the category 
            var existing = await repository.GetByIdAsyncWithoutTracking(
                category => category.NameAr == nameAr || category.NameEn == nameEn,
                cancellationToken);

            //. return conflict status code if the duplication is exist 
            if (existing is not null)
                return Failure(result, "A category with the same Arabic or English name already exists.", 409);

            var category = modelFactory.CreateCategory(nameAr,
                                                       nameEn,
                                                       request.requestDTO.DescriptionAr.Trim(),
                                                       request.requestDTO.DescriptionEn.Trim());

            category.CreatedByUserId = currentUserService.GetCurrentUserService()!.ToString() ?? "System";
            category.CreatedByIP = currentUserService.UserIP()!; //. must be existed (The user is logged in as an admin)

            await repository.AddAsync(category, cancellationToken);
            await unitOfWork.SavechangesAsync(cancellationToken);

            result.IsSuccess = true;
            result.Message = "Category created successfully.";
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
