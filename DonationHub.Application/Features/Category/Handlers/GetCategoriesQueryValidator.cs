using DonationHub.Application.Features.Category.Queries;
using FluentValidation;

namespace DonationHub.Application.Features.Category.Handlers
{

    public sealed class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
    {
        public GetCategoriesQueryValidator()
        {
            RuleFor(query => query.Page)
                .GreaterThan(0).WithMessage("The page number must be greater than zero.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, 100).WithMessage("The page size must be between 1 and 100.");

            RuleFor(query => query.SearchTerm)
                .MaximumLength(200).WithMessage("The search term must not exceed 200 characters.");
        }
    }
}
