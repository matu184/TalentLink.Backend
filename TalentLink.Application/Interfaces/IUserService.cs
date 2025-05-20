using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentLink.Domain.Entities;

namespace TalentLink.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(User user, string password);
    }
}
