using DonationHub.Application.Features.Auth.DTOs;
using FluentValidation;

namespace DonationHub.Application.Features.Auth.Handlers
{
    public sealed class LoginRequestCommandValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestCommandValidator()
        {
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is not valid.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
