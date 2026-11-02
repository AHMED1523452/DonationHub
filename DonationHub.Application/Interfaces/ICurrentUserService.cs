

namespace DonationHub.Application.Interfaces
{
    public interface ICurrentUserService
    {
       Guid GetCurrentUserService();
       
       string? UserIP();
       
       string? Email();
    }
}
