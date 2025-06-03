using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentLink.Application.Interfaces;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;

namespace TalentLink.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly TalentLinkDbContext _context;

        public UserService(TalentLinkDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            // Achtung: Nur zu Demozwecken – Passwort ist Klartext!
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            user.PasswordHash = password; // Hier später Hashing einbauen!
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
