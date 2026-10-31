
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly TimeProvider timeProvider;

        public UserService(UserManager<ApplicationUser> userManager,
                           RoleManager<IdentityRole<Guid>> roleManager,
                           TimeProvider timeProvider)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.timeProvider = timeProvider;
        }

        public async Task<bool> CreateUserAsync(string UserName,
                                                string FullName,
                                                string PreferredLanguage,
                                                string Email,
                                                string password,
                                                CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {
                UserName = UserName,
                FullName = FullName,
                preferredLanguage = PreferredLanguage,
                CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
                Email = Email
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return false;
            return true;
        }

        public async Task<object?> FindByEmailAsync(string Email,
                                                   CancellationToken cancellationToken = default)
        {
            return await userManager.FindByEmailAsync(Email);
        }
    }
}
