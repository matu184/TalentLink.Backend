using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TalentLink.Domain.Entities;

namespace TalentLink.Infrastructure.Persistence
{
    public static class UserSeeder
    {
        public static void SeedUsers(TalentLinkDbContext context)
        {
            if (context.Users.Any()) return;

            var users = new List<User>
        {
            new Senior
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Senior Max",
                Email = "senior@test.de",
                PasswordHash = Hash("123456"),
                Role = UserRole.Senior
            },
            new Student
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Student Lisa",
                Email = "student@test.de",
                PasswordHash = Hash("123456"),
                Role = UserRole.Student
            },
            new Parent
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Parent Julia",
                Email = "parent@test.de",
                PasswordHash = Hash("123456"),
                Role = UserRole.Parent,
                VerifiedStudents = new List<VerifiedStudent>()
            }
        };

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }

}
