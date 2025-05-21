using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Application.DTOs
{
    public class JobDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBoosted { get; set; }
        public string Category { get; set; } = null!;
        public string CategoryImage { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public List<JobCommentDto> Comments { get; set; } = new();
        public List<ApplicationInfoDto>? Applications { get; set; }

    }
    public class ApplicationInfoDto
    {
        public string StudentName { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
