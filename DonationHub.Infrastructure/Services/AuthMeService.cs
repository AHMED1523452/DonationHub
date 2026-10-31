using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using DonationHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DonationHub.Infrastructure.Services
{
    public  class AuthMeService : IAuthMeService
    {
        private readonly ICurrentUserService userService;
        private readonly ILogger<AuthMeService> logger;
        private readonly UserManager<ApplicationUser> userManager;

        public AuthMeService(ICurrentUserService userService,
                             ILogger<AuthMeService> logger,
                             UserManager<ApplicationUser> userManager)
        {
            this.userService = userService;
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<GenericResult<UserProfileResponseDTO>> Profile(CancellationToken cancellationToken = default)
        {
            var result = new GenericResult<UserProfileResponseDTO>();

            Guid? userId = userService.GetCurrentUserService();
            if(userId is null || userId == Guid.Empty)
            {
                result.Message = "User not authorized. ";
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status401Unauthorized;
                return result;
            }
            var user = await userManager.FindByIdAsync(userId.Value.ToString());
            if (user is null) {
                logger.LogWarning("There is something invalid occurred,the user has an id but does not have a record in db.");
                result.Message = "User profile not found. ";
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                return result;
            }


            result.Message = "User retrieved his profile";
            result.Data = new UserProfileResponseDTO
            {
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName
            };
            result.IsSuccess = true;
            return result;
        }
    }
}
