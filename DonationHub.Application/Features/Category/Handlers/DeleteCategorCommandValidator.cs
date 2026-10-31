using DonationHub.Application.Features.Category.Commands;
using FluentValidation;

namespace DonationHub.Application.Features.Category.Handlers
{
    public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("The category identifier is required.");
        }
    }
}
