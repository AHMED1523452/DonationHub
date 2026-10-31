using DonationHub.Application.Features.Category.Queries;
using FluentValidation;

namespace DonationHub.Application.Features.Category.Handlers
{
    public sealed class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(query => query.Id)
                .NotEmpty().WithMessage("The category identifier is required.");
        }
    }
}
