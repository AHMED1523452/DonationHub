using DonationHub.Application.Factories;
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Application.Patterns;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Services
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly TimeProvider timeProvider;
        private readonly IGenerateJwtToken jwtToken;
        private readonly IModelFactory modelFactory;
        private readonly IHashingService hashing;
        private readonly IOptions<JWTSettings> options;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUser;

        public LoginService(UserManager<ApplicationUser>userManager,
                            RoleManager<IdentityRole<Guid>> roleManager, 
                            TimeProvider timeProvider, 
                            IGenerateJwtToken jwtToken,
                            IModelFactory modelFactory,
                            IHashingService hashing, 
                            IOptions<JWTSettings> options,
                            IUnitOfWork unitOfWork,
                            ICurrentUserService currentUser)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.timeProvider = timeProvider;
            this.jwtToken = jwtToken;
            this.modelFactory = modelFactory;
            this.hashing = hashing;
            this.options = options;
            this.unitOfWork = unitOfWork;
            this.currentUser = currentUser;
        }
        public async Task<GenericResult<LoginResponseDTO>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default)
        {
            var result = new GenericResult<LoginResponseDTO>();

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status401Unauthorized;
                result.Message = "Invalid credentials.";
                return result;
            }

            var valid = await userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status401Unauthorized;
                result.Message = "Invalid credentials.";
                return result;
            }

            if (user.IsRevoked || !user.IsActive)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status403Forbidden;
                result.Message = "This account is not allowed to sign in.";
                return result;
            }

            // Require email confirmation
            if (!user.EmailConfirmed)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status403Forbidden;
                result.Message = "Email not verified. Please verify your email before logging in.";
                return result;
            }

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var access = await jwtToken.GenerateAccessTokenAsync(user.Id.ToString(), user.Email, user.UserName, user.FullName, role, cancellationToken);

            var refreshToken = await jwtToken.GenerateRefreshTokenAsync(user.Id.ToString(), user.Email, user.UserName, user.FullName, role, cancellationToken);

            var refreshTokenHashed = await hashing.HashTokenOrTextAsync(refreshToken, cancellationToken);

            var refreshEntity = modelFactory.CreateRefreshToken(user.Id.ToString(), refreshTokenHashed ?? throw new ArgumentNullException(nameof(refreshTokenHashed))
                                                                                  , null, timeProvider.GetUtcNow().UtcDateTime.AddDays(options.Value.RefreshTokenExpiryDays));

            await unitOfWork.Repository<RefreshTokens>().AddAsync(refreshEntity, cancellationToken);
            await unitOfWork.SavechangesAsync(cancellationToken);

            result.IsSuccess = true;
            result.Data = new LoginResponseDTO()
            {
                AccessToken = access,
                RefreshToken = $"{refreshEntity.Id}.{refreshToken}"
            };
            return result;
        }

        public async Task<GenericResult<string>> LogoutAsync(CancellationToken cancellationToken = default)
        {
            var result = new GenericResult<string>();

            var currentUserId = currentUser.GetCurrentUserService();
            if (currentUserId is null)
            {
                result.IsSuccess = false;
                result.Message = "The current user could not be identified.";
                return result;
            }

            var userId = currentUserId.Value.ToString();
            var revokedAt = timeProvider.GetUtcNow().UtcDateTime;

            //. avoding doing this approach using foreach or looping on the tokens that was not revoked for this user and revoke these toknes
            await unitOfWork.Repository<RefreshTokens>()
                                       .ExecuteEditAsync(token => token.UserId == userId && !token.IsRevoked,
                                                         setters => setters.SetProperty(token => token.IsRevoked, true)
                                                                           .SetProperty(token => token.RevokedAt, revokedAt),
                                                         cancellationToken);

            result.IsSuccess = true;
            result.Message = "Logged out successfully.";
            return result;
        }
    }
}
