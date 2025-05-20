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
            if (context.Jobs.Any()) return; // bereits vorhanden

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
                Category = JobCategory.Einkaufen,
                CreatedById = Guid.NewGuid() // später echte Senior-ID
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Nachhilfe Mathe 8. Klasse",
                Description = "Algebra und Bruchrechnung",
                PricePerHour = 15,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsBoosted = false,
                Category = JobCategory.Nachhilfe,
                CreatedById = Guid.NewGuid()
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Rasen mähen",
                Description = "1x pro Woche den Garten mähen",
                PricePerHour = 12,
                CreatedAt = DateTime.UtcNow,
                IsBoosted = false,
                Category = JobCategory.Gartenarbeit,
                CreatedById = Guid.NewGuid()
            }
        };

            context.Jobs.AddRange(jobs);
            context.SaveChanges();
        }
    }
}
