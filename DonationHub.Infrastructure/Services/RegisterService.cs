
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using DonationHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DonationHub.Infrastructure.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly TimeProvider timeProvider;
        private readonly IGenerateJwtToken jwtToken;
        private readonly ILogger<RegisterService> logger;

        public RegisterService(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole<Guid>> roleManager,
                               TimeProvider timeProvider,
                               IGenerateJwtToken jwtToken,
                               ILogger<RegisterService> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.timeProvider = timeProvider;
            this.jwtToken = jwtToken;
            this.logger = logger;
        }

        public async Task<GenericResult<RegisterResponseDTO>> RegisterDonorAsync(
            RegisterRequestDTO request,
            CancellationToken cancellationToken = default)
        {
            var result = new GenericResult<RegisterResponseDTO>();

            var existing = await userManager.FindByEmailAsync(request.Email);
            if (existing is not null)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status409Conflict;
                result.Message = "Email already registered.";
                return result;
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
                FullName = request.FullName,
                UserName = request.UserName,
                preferredLanguage = (request.PreferredLanguage ?? Domain.Enums.PreferredLanguage.en).ToString()
            };

            var create = await userManager.CreateAsync(user, request.Password);
            if (!create.Succeeded)
            {
                //. returning the description if the process not completed
                result.IsSuccess = false;
                result.Message = string.Join(";", create.Errors.Select(e => e.Description));
                return result;
            }

            //.Validation for existing role or not
            if (!await roleManager.RoleExistsAsync("Donor"))
            {
                logger.LogError("The role not found.");
                await userManager.DeleteAsync(user);
                result.IsSuccess = false;
                result.Message = $"The role is not configured.";
                return result;
            }

            var roleResult = await userManager.AddToRoleAsync(user, "Donor");
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                result.IsSuccess = false;
                result.Message = string.Join(";", roleResult.Errors.Select(e => e.Description));
                return result;
            }

            var accessToken = await jwtToken.GenerateAccessTokenAsync(user.Id.ToString(),
                                                                       request.Email,
                                                                       request.UserName,
                                                                       request.FullName,
                                                                       "Donor",
                                                                       cancellationToken);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                logger.LogError("Acess token wasn't created successfully.");
                await userManager.DeleteAsync(user);
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Message = $"Acess token wasn't created successfully.";
                return result;
            }

            result.IsSuccess = true;
            result.Message = "If the email you provided exists, a verification code has been sent. Please check your inbox.";
            result.Data = new RegisterResponseDTO()
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                AccessToken = accessToken
            };
            return result;
        }
    }
}
