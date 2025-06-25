using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentLink.Application.DTOs
{
    public class CreateRatingDto
    {
        public Guid ToUserId { get; set; } // Zielperson
        public int Score { get; set; }     // z. B. 1–5
        public string? Comment { get; set; }
    }
}
