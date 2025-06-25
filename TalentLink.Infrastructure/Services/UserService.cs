using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentLink.Application.Interfaces;
using TalentLink.Domain.Entities;
using TalentLink.Infrastructure.Persistence;
<<<<<<< HEAD
using BCrypt.Net;
=======
>>>>>>> heroku/main

namespace TalentLink.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly TalentLinkDbContext _context;

        public UserService(TalentLinkDbContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
        public async Task<User?> AuthenticateAsync(string email, string password, string zipCode, string city)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }
            return null;
=======
        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            // Achtung: Nur zu Demozwecken – Passwort ist Klartext!
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
>>>>>>> heroku/main
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
<<<<<<< HEAD
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
=======
            user.PasswordHash = password; // Hier später Hashing einbauen!
>>>>>>> heroku/main
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
