using DonationHub.Application.Features.Category.DTOs;
using FluentValidation;


namespace DonationHub.Application.Features.Category.Handlers
{
    public sealed class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequestDTO>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(request => request.NameAr)
                .NotEmpty().WithMessage("The Arabic name is required.")
                .MaximumLength(100).WithMessage("The Arabic name must not exceed 100 characters.");

            RuleFor(request => request.NameEn)
                .NotEmpty().WithMessage("The English name is required.")
                .MaximumLength(100).WithMessage("The English name must not exceed 100 characters.");

            RuleFor(request => request.DescriptionAr)
                .NotEmpty().WithMessage("The Arabic description is required.")
                .MaximumLength(1000).WithMessage("The Arabic description must not exceed 1000 characters.");

            RuleFor(request => request.DescriptionEn)
                .NotEmpty().WithMessage("The English description is required.")
                .MaximumLength(1000).WithMessage("The English description must not exceed 1000 characters.");
        }
    }
}
