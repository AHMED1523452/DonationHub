

using DonationHub.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DonationHub.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor accessor;

        public CurrentUserService(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }
        public Guid? GetCurrentUserService()
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var result) == false ?
                                    null
                                    : result;
        }

        public string? UserIP() => accessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        public string? Email()
        {
            string userEmail = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
            return userEmail == null ? null : userEmail;
        }
    }
}
