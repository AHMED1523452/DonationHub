
using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DonationHub.Infrastructure.Services
{
    public class CreateAdminOrStaffService : ICreateAdminOrStaffService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly TimeProvider timeProvider;
        private readonly ILogger<CreateAdminOrStaffService> logger;

        public CreateAdminOrStaffService(UserManager<ApplicationUser> userManager,
                                         RoleManager<IdentityRole<Guid>> roleManager,
                                         TimeProvider timeProvider,
                                         ILogger<CreateAdminOrStaffService> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.timeProvider = timeProvider;
            this.logger = logger;
        }

        public async Task<GenericResult<CreateAdminOrStaffResponseDTO>> CreateAsync(
            CreateAdminOrStaffCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = new GenericResult<CreateAdminOrStaffResponseDTO>();

            //. first we will valdiate the duplication (it's impossible to be not null)
            if (await userManager.FindByEmailAsync(command.Email) is not null)
                return Failure(result, "Email is already registered.", StatusCodes.Status409Conflict);

            //. validating duplication for the user name (should be existing in front too) 
            if (await userManager.FindByNameAsync(command.UserName) is not null)
                return Failure(result, "User name is already taken.", StatusCodes.Status409Conflict);

            //. validating if the role that the admin provided is existing 
            var role = await roleManager.FindByNameAsync(command.Role.ToString());
            if (role is null)
            {
                logger.LogError("The role {Role} is not configured.", command.Role);
                return Failure(result, $"The role '{command.Role}' is not configured.", StatusCodes.Status400BadRequest);
            }

            var language = command.PreferredLanguage ?? PreferredLanguage.en;

            var user = new ApplicationUser
            {
                UserName = command.UserName,
                FullName = command.FullName,
                Email = command.Email,
                preferredLanguage = language.ToString(),
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = timeProvider.GetUtcNow().UtcDateTime
            };

            var createResult = await userManager.CreateAsync(user, command.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
                return Failure(result, errors, StatusCodes.Status400BadRequest);
            }

            var roleResult = await userManager.AddToRoleAsync(user, role.Name!);
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                var errors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
                return Failure(result, errors, StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("Created the {Role} account {Email}.", role.Name, command.Email);

            result.IsSuccess = true;
            result.Message = $"The {role.Name} account was created successfully.";
            result.Data = new CreateAdminOrStaffResponseDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                Role = role.Name,
                PreferredLanguage = user.preferredLanguage
            };

            return result;
        }

        private static GenericResult<CreateAdminOrStaffResponseDTO> Failure(
            GenericResult<CreateAdminOrStaffResponseDTO> result,
            string message,
            int statusCode)
        {
            result.IsSuccess = false;
            result.Message = message;
            result.StatusCode = statusCode;
            return result;
        }
    }
}
