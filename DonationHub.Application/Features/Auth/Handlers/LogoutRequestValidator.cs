using DonationHub.Application.Features.Auth.DTOs;
using FluentValidation;


namespace DonationHub.Application.Features.Auth.Handlers
{
    public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequestDTO>
    {
        public LogoutRequestValidator()
        {
            RuleFor(request => request.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
