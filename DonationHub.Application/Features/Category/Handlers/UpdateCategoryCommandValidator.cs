
using DonationHub.Application.Features.Category.Commands;
using FluentValidation;

namespace DonationHub.Application.Features.Category.Handlers
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("The category identifier is required.");

            RuleFor(command => command.requestDTO)
                .NotNull().WithMessage("The category payload is required.")
                .SetValidator(new UpdateCategoryRequestValidator());
        }
    }
}
