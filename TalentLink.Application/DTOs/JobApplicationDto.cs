using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Application.DTOs
{
    public class JobApplicationDto
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string? Message { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
