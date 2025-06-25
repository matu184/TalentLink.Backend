using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentLink.Domain.Entities;

namespace TalentLink.Application.DTOs
{
    public class CreateJobDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public JobCategory Category { get; set; }
        public decimal PricePerHour { get; set; }
        public bool IsBoosted { get; set; }
        public string? ZipCode { get; set; }
        public string? City { get; set;  }
        public int MinimumAge { get; set;  }
    }
}
