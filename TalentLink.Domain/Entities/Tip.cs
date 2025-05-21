using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Domain.Entities
{
    public class Tip
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;
    }
}
