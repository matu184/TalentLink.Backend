using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Application.DTOs
{
    public class JobCommentDto
    {
        public string AuthorName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
