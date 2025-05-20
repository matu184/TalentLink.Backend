using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentLink.Domain.Enums;

namespace TalentLink.Domain.Entities
{
    public class JobApplication
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;

        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public string? Message { get; set; } // optional
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    }
}
