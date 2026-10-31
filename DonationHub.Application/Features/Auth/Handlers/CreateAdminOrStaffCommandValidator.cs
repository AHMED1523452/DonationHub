using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Domain.Enums;
using FluentValidation;
namespace DonationHub.Application.Features.Auth.Handlers
{
    public sealed class CreateAdminOrStaffCommandValidator : AbstractValidator<CreateAdminOrStaffCommand>
    {
        private static readonly string[] AllowedRoles = ["Admin", "CampaignManager"];

        public CreateAdminOrStaffCommandValidator()
        {
            RuleFor(command => command.UserName)
                .NotEmpty().WithMessage("User name is required.")
                .MinimumLength(3).WithMessage("User name must be at least 3 characters.")
                .MaximumLength(50).WithMessage("User name must not exceed 50 characters.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("User name can contain letters, digits and underscores only.");

            RuleFor(command => command.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

            RuleFor(command => command.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is not valid.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

            RuleFor(command => command.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(command => command.ConfirmPassword)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .Must((command, confirmPassword) => confirmPassword == command.Password)
                .WithMessage("Password confirmation does not match the password.");

            RuleFor(command => command.Role)
                .NotEmpty().WithMessage("Role is required.")
                .WithMessage("Role must be either Admin or CampaignManager.");

            RuleFor(command => command.PreferredLanguage)
                .Must(language => !language.HasValue || Enum.IsDefined(language.Value))
                .WithMessage("Preferred language must be either en or ar.");
        }
    }
}
