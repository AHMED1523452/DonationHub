
namespace DonationHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(string UserName,
                                   string FullName,
                                   string PreferredLanguage,
                                   string Email,
                                   string password,
                                   CancellationToken cancellationToken = default);

        Task<object?> FindByEmailAsync(string Email,
                                      CancellationToken cancellationToken = default);
    }
}
