using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentLink.Domain.Entities;

namespace TalentLink.Infrastructure.Persistence
{
    public static class JobSeeder
    {
        public static void Seed(TalentLinkDbContext context)
        {
            if (context.Jobs.Any()) return;

            var jobs = new List<Job>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Einkaufen für Oma",
                Description = "Brauche Hilfe beim Wocheneinkauf.",
                PricePerHour = 10,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                IsBoosted = true,
                CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000004"), // Einkaufen
                CreatedById = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Nachhilfe Mathe 8. Klasse",
                Description = "Algebra und Bruchrechnung",
                PricePerHour = 15,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsBoosted = false,
                CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000003"), // Nachhilfe
                CreatedById = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Rasen mähen",
                Description = "1x pro Woche den Garten mähen",
                PricePerHour = 12,
                CreatedAt = DateTime.UtcNow,
                IsBoosted = false,
                CategoryId = Guid.Parse("10000000-0000-0000-0000-000000000001"), // Gartenarbeit
                CreatedById = Guid.Parse("11111111-1111-1111-1111-111111111111")
            }
        };

            context.Jobs.AddRange(jobs);
            context.SaveChanges();
        }
    }
}

