using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Domain.Entities
{
    public class Rating
    {
        public Guid Id { get; set; }

        public Guid FromUserId { get; set; }
        public User FromUser { get; set; } = null!;

        public Guid ToUserId { get; set; }
        public User ToUser { get; set; } = null!;

        public int Score { get; set; } // 1–5
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
