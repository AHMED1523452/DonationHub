using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces
{
    public interface IRoleService
    {
        Task<bool> RoleExists(string role);
    }
}
