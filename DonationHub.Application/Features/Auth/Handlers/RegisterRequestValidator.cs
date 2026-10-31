
using DonationHub.Application.Features.Auth.DTOs;
using FluentValidation;

namespace DonationHub.Application.Features.Auth.Handlers
{
    public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestValidator()
        {
            RuleFor(request => request.UserName)
                .NotEmpty().WithMessage("User name is required.")
                .MinimumLength(3).WithMessage("User name must be at least 3 characters.")
                .MaximumLength(50).WithMessage("User name must not exceed 50 characters.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("User name can contain letters, digits and underscores only.");

            RuleFor(request => request.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(request => request.ConfirmPassword)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .Must((request, confirmPassword) => confirmPassword == request.Password)
                .WithMessage("Password confirmation does not match the password.");

            RuleFor(request => request.PreferredLanguage)
                .Must(language => !language.HasValue || Enum.IsDefined(language.Value))
                .WithMessage("Preferred language must be either en or ar.");
        }
    }
}
