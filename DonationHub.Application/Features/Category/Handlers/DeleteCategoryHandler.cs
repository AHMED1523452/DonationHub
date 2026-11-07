using DonationHub.Application.Features.Category.Commands;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using DonationHub.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.Application.Features.Category.Handlers
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, GenericResult<string>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly TimeProvider timeProvider;

        public DeleteCategoryHandler(IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            this.unitOfWork = unitOfWork;
            this.timeProvider = timeProvider;
        }

        public async Task<GenericResult<string>> Handle(DeleteCategoryCommand request,
                                                        CancellationToken cancellationToken)
        {
            var result = new GenericResult<string>();
            var repository = unitOfWork.Repository<CategoryEntity>();

            //.getting the Category with tracking to delete process (soft)
            var category = await repository.GetTrackedAsync(
                item => item.Id == request.Id,
                false,
                cancellationToken);

            if (category is null)
            {
                result.IsSuccess = false;
                result.Message = "Category not found.";
                result.StatusCode = 404;
                return result;
            }

            var campaignsCount = await unitOfWork.Repository<Campaign>()
                            .CountAsync(op => op.CategoryId == request.Id, cancellationToken);
            if (campaignsCount > 0)
            {
                result.IsSuccess = false;
                result.Message = $"This category is linked to {campaignsCount} campaign(s) and cannot be deleted.";
                result.StatusCode = 409;
                return result;
            }

            category.IsDeleted = true;
            category.DeletedAt = timeProvider.GetUtcNow().UtcDateTime;

            await unitOfWork.SavechangesAsync(cancellationToken);

            result.IsSuccess = true;
            result.Message = "Category deleted successfully.";
            result.Data = category.Id.ToString();
            return result;
        }
    }
}
