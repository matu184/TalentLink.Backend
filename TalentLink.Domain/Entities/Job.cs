using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Domain.Entities
{
    public class Job
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsBoosted { get; set; }
        public bool IsPaid { get; set; } = false;
        public string? ZipCode { get; set; }
        public string? City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid CategoryId { get; set; }
        public JobCategory Category { get; set; } = null!;
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public bool IsAssigned { get; set; } = false;
        public ICollection<JobComment> Comments { get; set; } = new List<JobComment>();
        public int? MinimumAge { get; set; } // optional, falls nicht gesetzt



    }
}
