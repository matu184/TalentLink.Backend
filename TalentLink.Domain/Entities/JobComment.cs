using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Domain.Entities
{
    public class JobComment
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;

        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
