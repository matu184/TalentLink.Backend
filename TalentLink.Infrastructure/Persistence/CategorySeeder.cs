using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentLink.Domain.Entities;

namespace TalentLink.Infrastructure.Persistence
{
    public static class CategorySeeder
    {
        public static void SeedCategories(TalentLinkDbContext context)
        {
            if (context.JobCategories.Any()) return;

            var categories = new List<JobCategory>
        {
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "Gartenarbeit",
                ImageUrl = "/images/categories/Gartenarbeit.png"
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Babysitting",
                ImageUrl = "/images/categories/Babysitting.png"
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "Nachhilfe",
                ImageUrl = "/images/categories/Nachhilfe.png"
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name = "Einkaufen",
                ImageUrl = "/images/categories/Einkaufen.png"
            },
            new()
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Name = "Haushalt",
                ImageUrl = "/images/categories/Haushalt.png"
            }
        };

            context.JobCategories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
