
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
    public class RefreshTokenService : IRefreshTokenService
    { 
        private readonly IUnitOfWork unitOfWork;
        private readonly IHashingService hashing;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IGenerateJwtToken jwtToken;
        private readonly IModelFactory factory;
        private readonly TimeProvider timeProvider;
        private readonly IOptions<JWTSettings> options;

        public RefreshTokenService(IUnitOfWork  unitOfWork, 
                                   IHashingService hashing,
                                   UserManager<ApplicationUser> userManager, 
                                   IGenerateJwtToken jwtToken,
                                   IModelFactory factory, 
                                   TimeProvider timeProvider,
                                   IOptions<JWTSettings> options)
        {
            this.unitOfWork = unitOfWork;
            this.hashing = hashing;
            this.userManager = userManager;
            this.jwtToken = jwtToken;
            this.factory = factory;
            this.timeProvider = timeProvider;
            this.options = options;
        }
        public async Task<GenericResult<RefreshTokenRequestDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken)
        {
            var result = new GenericResult<RefreshTokenRequestDTO >();
            var parts = request.refreshToken?.Split('.', 2); ///. the first part is the id for the token

            if (parts is null ||
                         parts.Length != 2 ||
                        !Guid.TryParse(parts[0], out var tokenId) ||
                        string.IsNullOrWhiteSpace(parts[1]))
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status401Unauthorized;
                result.Message = "Invalid refresh token.";
                return result;
            }

            var stored = await unitOfWork.Repository<RefreshTokens>()
                .GetByIdAsyncWithoutTracking(t => t.Id == tokenId, cancellationToken);

            if (stored is null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status401Unauthorized;
                result.Message = "Invalid or expired refresh token.";
                return result;
            }

            var isValid = await hashing.VerifyAsync(stored.TokenHash, parts[1], cancellationToken);
            if (!isValid)
            {
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status401Unauthorized;
                result.Message = "Invalid refresh token.";
                return result;
            }

            stored.IsRevoked = true;
            stored.RevokedAt = DateTime.UtcNow;

            var user = await userManager.FindByIdAsync(stored.UserId.ToString());
            if (user is null || !user.IsActive || user.IsRevoked)
            {
                await unitOfWork.Repository<RefreshTokens>().EditAsync(stored, cancellationToken);
                await unitOfWork.SavechangesAsync(cancellationToken);
                result.IsSuccess = false;
                result.StatusCode = StatusCodes.Status401Unauthorized;
                result.Message = "Invalid refresh token.";
                return result;
            }

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var newRaw = await jwtToken.GenerateRefreshTokenAsync(user.Id.ToString(), user.Email!, user.UserName, user.FullName, role, cancellationToken);
            var hashed = await hashing.HashTokenOrTextAsync(newRaw, cancellationToken);

            var newRefresh = factory.CreateRefreshToken(user.Id.ToString(), hashed, tokenId.ToString(), timeProvider.GetUtcNow().UtcDateTime.AddDays(options.Value.RefreshTokenExpiryDays));


            //. First update the record of the stored refresh token or the old  refresh token as revoked 
            await unitOfWork.Repository<RefreshTokens>().EditAsync(stored, cancellationToken);
            
            //. after thta add the new refresh token as a new record
            await unitOfWork.Repository<RefreshTokens>().AddAsync(newRefresh, cancellationToken);
            await unitOfWork.SavechangesAsync(cancellationToken);

            result.IsSuccess = true;
            result.Data = new RefreshTokenRequestDTO()
            {
                refreshToken = newRefresh.Id + "." + newRaw
            };
            return result;
        }
    }
}
